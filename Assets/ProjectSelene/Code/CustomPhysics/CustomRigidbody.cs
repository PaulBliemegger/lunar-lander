// CustomRigidbody.cs
using System;
using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    [DefaultExecutionOrder(-50)]
    [RequireComponent(typeof(Collider))]
    public class CustomRigidbody : MonoBehaviour
    {
        [Header("Motion")]
        public float mass = 1f;
        public float linearDamping = 0.05f;
        public Vector3 Velocity { get; set; }
        public float Speed => Velocity.magnitude;

        [Header("Collision")]
        public bool useCollisions = true;
        [SerializeField] public LayerMask collisionLayers = ~0;
        public float skinWidth = 0.01f;
        public int maxSweepSteps = 3;

        // Plug a solver here (AabbCollisionSolver or UnityCastCollisionSolver)
        [SerializeField] private MonoBehaviour collisionSolverComponent; // must implement ICollisionSolver
        private ICollisionSolver _solver;

        // Telemetry
        public bool IsGrounded { get; private set; }
        public float LastImpactSpeed { get; private set; }
        public Vector3 LastImpactVelocity { get; private set; }
        public Vector3 LastImpactNormal { get; private set; } = Vector3.up;

        /// Fired on first contact in a step (other collider only).
        public event Action<Collider> OnCollision;

        // Internals
        private Vector3 _forceAccum;
        private Collider _self;

        void Awake()
        {
            _self = GetComponent<Collider>();

            if (collisionSolverComponent is ICollisionSolver solver)
            {
                _solver = solver;
                solver.Initialize(this);
            }
            else if (collisionSolverComponent != null)
            {
                Debug.LogError($"{name}: Assigned solver does not implement ICollisionSolver.");
            }
        }

        // Allow swapping solvers at runtime
        public void SetCollisionSolver(ICollisionSolver solver)
        {
            _solver = solver;
            _solver?.Initialize(this);
        }

        public void SetVelocity(Vector3 v) => Velocity = v;
        public void AddForce(Vector3 force) => _forceAccum += force;

        public void AddImpulse(Vector3 impulse)
        {
            if (mass > 0f) Velocity += impulse / mass;
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (dt <= 0f || mass <= 0f) { _forceAccum = Vector3.zero; return; }
            
            Vector3 acc = _forceAccum / mass;
            Velocity += acc * dt;
            
            if (linearDamping > 0f)
                Velocity *= Mathf.Clamp01(1f - linearDamping * dt);
            
            if (!useCollisions || _solver == null)
            {
                transform.position += Velocity * dt;
                IsGrounded = false;
            }
            else
            {
                Vector3 pos = transform.position;
                Vector3 vel = Velocity;

                _solver.Solve(ref pos, ref vel, dt, out var res);

                transform.position = pos;
                Velocity = vel;
                
                IsGrounded = res.grounded;
                if (res.hit)
                {
                    LastImpactVelocity = res.impactVelocity;
                    LastImpactSpeed = res.impactVelocity.magnitude;
                    LastImpactNormal = res.normal;
                    OnCollision?.Invoke(res.other);
                }
            }

            _forceAccum = Vector3.zero;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + Velocity);
        }
#endif
    }
}
