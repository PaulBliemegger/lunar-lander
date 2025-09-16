using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public class UnityCastCollisionSolver : MonoBehaviour, ICollisionSolver
    {
        [SerializeField] private LayerMask layers = ~0;
        [SerializeField] private float skinWidth = 0.01f;
        [SerializeField] private int maxSweepSteps = 3;

        private CustomRigidbody _rb;
        private Collider _self;

        public void Initialize(CustomRigidbody rb)
        {
            _rb = rb;
            _self = rb.GetComponent<Collider>();
            layers = rb.collisionLayers;
            skinWidth = rb.skinWidth;
            maxSweepSteps = rb.maxSweepSteps;

            if (_self.isTrigger) _self.isTrigger = false; // must collide
        }

        public void Solve(ref Vector3 position, ref Vector3 velocity, float dt, out CollisionResult res)
        {
            res = default;
            bool grounded = false;

            Vector3 remaining = velocity * dt;

            for (int step = 0; step < maxSweepSteps; step++)
            {
                if (remaining.sqrMagnitude < 1e-10f) break;

                Vector3 dir = remaining.normalized;
                float dist = remaining.magnitude + skinWidth;

                bool hit;
                RaycastHit h;

                if (_self is BoxCollider box)
                {
                    Vector3 half; Quaternion rot;
                    GetBoxWorld(box, out Vector3 center, out half, out rot);

                    Vector3 castOrigin = center + (position - _self.transform.position);
                    hit = Physics.BoxCast(castOrigin, half, dir, out h, rot, dist, layers, QueryTriggerInteraction.Ignore);
                }
                else if (_self is SphereCollider sphere)
                {
                    float r = GetSphereWorldRadius(sphere);
                    Vector3 c = sphere.transform.TransformPoint(sphere.center) + (position - _self.transform.position);
                    hit = Physics.SphereCast(c, r, dir, out h, dist, layers, QueryTriggerInteraction.Ignore);
                }
                else
                {
                    hit = Physics.Raycast(position, dir, out h, dist, layers, QueryTriggerInteraction.Ignore);
                }

                if (!hit)
                {
                    position += remaining;
                    break;
                }

                // move to just before impact
                float travel = Mathf.Max(0f, h.distance - skinWidth);
                position += dir * travel;

                // fill result for first contact this step
                if (!res.hit)
                {
                    res.hit = true;
                    res.other = h.collider;
                    res.normal = h.normal;
                    res.toi = (dist > 1e-6f) ? (travel / dist) : 0f;
                    res.impactVelocity = velocity;
                }

                // slide (remove into-normal component)
                float vn = Vector3.Dot(velocity, h.normal);
                if (vn < 0f) velocity -= vn * h.normal;

                // grounded?
                if (Vector3.Angle(h.normal, Vector3.up) < 60f) grounded = true;

                // nudge out
                position += h.normal * skinWidth;

                // continue remainder
                float used = travel / dist;
                float remainFrac = Mathf.Clamp01(1f - used);
                remaining = velocity * (dt * remainFrac);
            }

            res.grounded = grounded;
        }

        // Helpers
        static void GetBoxWorld(BoxCollider bc, out Vector3 center, out Vector3 halfExtents, out Quaternion rot)
        {
            var t = bc.transform;
            center = t.TransformPoint(bc.center);
            Vector3 s = bc.size * 0.5f;
            Vector3 lossy = t.lossyScale;
            halfExtents = new Vector3(Mathf.Abs(s.x * lossy.x), Mathf.Abs(s.y * lossy.y), Mathf.Abs(s.z * lossy.z));
            rot = t.rotation;
        }

        static float GetSphereWorldRadius(SphereCollider sc)
        {
            Vector3 ls = sc.transform.lossyScale;
            return sc.radius * Mathf.Max(Mathf.Abs(ls.x), Mathf.Abs(ls.y), Mathf.Abs(ls.z));
        }
    }
}
