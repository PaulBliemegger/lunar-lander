using System.Collections.Generic;
using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public class AabbCollisionSolver : MonoBehaviour, ICollisionSolver
    {
        [SerializeField] private LayerMask layers = ~0;
        [SerializeField] private float skinWidth = 0.01f;
        [SerializeField] private int maxSweepSteps = 3;

        private CustomRigidbody _rb;
        private Collider _self;
        private readonly List<Collider> _obstacles = new();

        public void Initialize(CustomRigidbody rb)
        {
            _rb = rb;
            _self = rb.GetComponent<Collider>();
            layers = rb.collisionLayers;
            skinWidth = rb.skinWidth;
            maxSweepSteps = rb.maxSweepSteps;

            RebuildObstacleList();
        }

        public void RebuildObstacleList()
        {
            _obstacles.Clear();
            var all = FindObjectsOfType<Collider>(includeInactive: false);
            foreach (var c in all)
            {
                if (c == _self) continue;
                if (!c.enabled || !c.gameObject.activeInHierarchy) continue;
                if (!(c is BoxCollider || c is SphereCollider)) continue;
                if (((1 << c.gameObject.layer) & layers.value) == 0) continue;
                _obstacles.Add(c);
            }
        }

        Bounds GetSelfAABB()
        {
            if (_self is BoxCollider sb) return CustomColliderUtility.GetAABB(sb);
            if (_self is SphereCollider ss) return CustomColliderUtility.GetAABB(ss);
            return new Bounds(_self.transform.position, Vector3.zero);
        }

        public void Solve(ref Vector3 position, ref Vector3 velocity, float dt, out CollisionResult res)
        {
            res = default;
            bool grounded = false;

            Vector3 remaining = velocity * dt;
            Bounds selfAabb = GetSelfAABB();
            selfAabb.center = position; // align with incoming position

            for (int step = 0; step < maxSweepSteps; step++)
            {
                if (remaining.sqrMagnitude < 1e-10f) break;

                Vector3 start = selfAabb.center;
                Vector3 v = remaining;

                float bestTOI = 1f;
                Vector3 bestNormal = Vector3.zero;
                Collider bestHit = null;

                foreach (var obs in _obstacles)
                {
                    if (!obs || !obs.enabled || !obs.gameObject.activeInHierarchy) continue;

                    Bounds target = (obs is BoxCollider ob)
                        ? CustomColliderUtility.GetAABB(ob)
                        : CustomColliderUtility.GetAABB((SphereCollider)obs);

                    Bounds expanded = new Bounds(target.center, target.size + selfAabb.size);

                    if (CustomColliderUtility.RayAABB(start, v, expanded, out float tEntry, out Vector3 n))
                    {
                        if (tEntry >= 0f && tEntry < bestTOI)
                        {
                            bestTOI = tEntry;
                            bestNormal = n;
                            bestHit = obs;
                        }
                    }
                }

                if (bestHit == null)
                {
                    position += remaining;
                    break;
                }

                // move to just before impact
                float pre = Mathf.Max(0f, bestTOI - CustomColliderUtility.EpsilonFor(v));
                position += v * pre;

                // impact
                res.hit = true;
                res.other = bestHit;
                res.normal = bestNormal;
                res.toi = bestTOI;
                res.impactVelocity = velocity;

                // slide
                float vn = Vector3.Dot(velocity, bestNormal);
                if (vn < 0f) velocity -= vn * bestNormal;

                // grounded?
                if (Vector3.Angle(bestNormal, Vector3.up) < 60f) grounded = true;

                // small depenetration
                position += bestNormal * skinWidth;

                // continue remainder
                float remainFrac = Mathf.Clamp01(1f - bestTOI);
                remaining = velocity * (dt * remainFrac);

                // refresh aabb center
                selfAabb = GetSelfAABB();
                selfAabb.center = position;
            }

            res.grounded = grounded;
        }
    }
}
