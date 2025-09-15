using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public struct CollisionResult
    {
        public bool hit;
        public bool grounded;
        public Collider other;
        public Vector3 normal;
        public Vector3 impactVelocity;
        public float toi; // 0..1 fraction of this step used before impact
    }

    public interface ICollisionSolver
    {
        // Called once so the solver can cache references it needs.
        void Initialize(CustomRigidbody rb);

        // Move/resolve for this tick. You must:
        // - Update position & velocity in-place (refs)
        // - Fill result with first hit (if any)
        void Solve(ref Vector3 position, ref Vector3 velocity, float dt, out CollisionResult result);
    }
}