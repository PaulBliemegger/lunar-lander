using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public class SimpleCustomRigidbody : MonoBehaviour
    {
        [Header("Motion")]
        public float mass = 1f;
        public float linearDamping = 0.05f;
        public Vector3 Velocity { get; set; }
        public float Speed => Velocity.magnitude; 
        
        [Header("Collision")]
        public bool useCollisions = true;  
        public float skinWidth = 0.02f;         // how far to stay off surfaces
        public int maxSweepSteps = 3;           // solve multiple contacts
        [Range(0f, 1f)] public float bounciness = 0f; // 0=slide, 1=perfect bounce
        
        public bool IsGrounded { get; private set; }
        public float LastImpactSpeed { get; private set; }       // magnitude at impact
        public float LastImpactNormalSpeed { get; private set; } // into-surface component
        public Vector3 LastImpactNormal { get; private set; } = Vector3.up;
        
        
        private Vector3 _forceAccum;
        private Collider _col;
        
        void Awake()
        {
            _col = GetComponent<Collider>();
            if (_col == null)
                Debug.LogError($"{name}: CustomRigidbody needs a Collider.");
        }

        /// <summary>Adds a force (N). Accumulates until next FixedUpdate.</summary>
        public void AddForce(Vector3 force)
        {
            _forceAccum += force;
        } 

        /// <summary>Immediate velocity change: momentum impulse (N·s).</summary>
        public void AddImpulse(Vector3 impulse)
        {
            if (mass <= 0f) return;
            Velocity += impulse / mass;
        }
/**
        private void IntegrateWithCollisions(float dt)
        {
            IsGrounded = false;
            Vector3 remaining = Velocity * dt;

            for (int step = 0; step < maxSweepSteps && remaining.sqrMagnitude > 1e-8f; step++)
            {
                Vector3 dir = remaining.normalized;
                float dist = remaining.magnitude + skinWidth;

                if (!_col.Cast(dir, out RaycastHit hit, dist))
                {
                    transform.position += remaining;
                    remaining = Vector3.zero;
                    break;
                }

                // Move to just before impact
                float travel = Mathf.Max(0f, hit.distance - skinWidth);
                transform.position += dir * travel;

                // Impact telemetry
                Vector3 n = hit.normal;
                float vMag = Velocity.magnitude;
                float vn = Vector3.Dot(Velocity, n); // into-surface is negative
                LastImpactSpeed = vMag;
                LastImpactNormalSpeed = Mathf.Max(0f, -vn);
                LastImpactNormal = n;
                //OnSurfaceHit?.Invoke(hit, vMag);

                // Resolve: bounce or slide
                if (vn < 0f)
                {
                    if (bounciness > 0f)
                    {
                        Velocity = Velocity - (1f + bounciness) * vn * n; // reflect
                    }
                    else
                    {
                        Velocity -= vn * n; // remove into-normal component (slide)
                    }
                }

                // Small depenetration & grounded flag
                transform.position += n * (skinWidth * 0.5f);
                IsGrounded = Vector3.Angle(n, Vector3.up) < 60f;

                // continue with new velocity
                remaining = Velocity * dt;
            }
        }
**/
        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            if (dt <= 0f || mass <= 0f) { _forceAccum = Vector3.zero; return; }

            // 1) Acceleration from forces
            Vector3 acc = _forceAccum / mass;
            //if (useGravity) acc += gravity;

            // 2) Semi-implicit Euler: v += a·dt, x += v·dt
            Velocity += acc * dt;

            // 3) Damping (exponential-ish; keeps units reasonable)
            if (linearDamping > 0f)
                Velocity *= Mathf.Clamp01(1f - linearDamping * dt);

            // 4) Move with collision using collider sweeps
            //IntegrateWithCollisions(Velocity, dt);
            if (useCollisions && _col != null)
            {
                
            //IntegrateWithCollisions(dt);
            }
            else
            {
                
                transform.position += Velocity * dt;
            }
            
            // 5) Clear forces for next tick
            _forceAccum = Vector3.zero;
        }
    }
}