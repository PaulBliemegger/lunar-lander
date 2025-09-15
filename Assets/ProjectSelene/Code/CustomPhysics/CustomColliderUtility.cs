using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public static class CustomColliderUtility
    {
        public static Bounds GetAABB(BoxCollider bc)
        {
            var t = bc.transform;
            Vector3 localHalf = bc.size * 0.5f;
            // World vectors of local axes scaled by half-sizes
            Vector3 ex = t.TransformVector(new Vector3(localHalf.x, 0, 0));
            Vector3 ey = t.TransformVector(new Vector3(0, localHalf.y, 0));
            Vector3 ez = t.TransformVector(new Vector3(0, 0, localHalf.z));
            Vector3 center = t.TransformPoint(bc.center);
            Vector3 ext = new Vector3(
                Mathf.Abs(ex.x) + Mathf.Abs(ey.x) + Mathf.Abs(ez.x),
                Mathf.Abs(ex.y) + Mathf.Abs(ey.y) + Mathf.Abs(ez.y),
                Mathf.Abs(ex.z) + Mathf.Abs(ey.z) + Mathf.Abs(ez.z)
            );
            return new Bounds(center, ext * 2f);
        }

        public static Bounds GetAABB(SphereCollider sc)
        {
            var t = sc.transform;
            Vector3 center = t.TransformPoint(sc.center);
            // Handle non-uniform scale conservatively by taking the max axis scale
            Vector3 lossy = t.lossyScale;
            float maxScale = Mathf.Max(Mathf.Abs(lossy.x), Mathf.Abs(lossy.y), Mathf.Abs(lossy.z));
            float r = Mathf.Abs(sc.radius * maxScale);
            Vector3 ext = Vector3.one * r;
            return new Bounds(center, ext * 2f);
        }
        
        // Ray (p -> p+v) vs AABB (slab method). Returns hit within [0,1]
        public static bool RayAABB(Vector3 p, Vector3 v, Bounds b, out float tEntry, out Vector3 normal)
        {
            tEntry = 0f; float tExit = 1f;
            normal = Vector3.zero;

            if (v.sqrMagnitude < 1e-12f) return false;

            Vector3 min = b.min, max = b.max;
            int entryAxis = -1;

            for (int axis = 0; axis < 3; axis++)
            {
                float p0 = axis == 0 ? p.x : (axis == 1 ? p.y : p.z);
                float d  = axis == 0 ? v.x : (axis == 1 ? v.y : v.z);
                float mn = axis == 0 ? min.x : (axis == 1 ? min.y : min.z);
                float mx = axis == 0 ? max.x : (axis == 1 ? max.y : max.z);

                if (Mathf.Abs(d) < 1e-12f)
                {
                    if (p0 < mn || p0 > mx) return false; // outside slab, no hit
                    continue;
                }

                float t1 = (mn - p0) / d;
                float t2 = (mx - p0) / d;
                if (t1 > t2) { float tmp = t1; t1 = t2; t2 = tmp; }

                if (t1 > tEntry) { tEntry = t1; entryAxis = axis; }
                if (t2 < tExit)  { tExit  = t2; }
                if (tEntry > tExit) return false;
            }

            if (tEntry < 0f || tEntry > 1f) return false;

            // Normal from entry axis and direction
            if (entryAxis == 0) normal = (v.x > 0f) ? Vector3.left  : Vector3.right;
            else if (entryAxis == 1)   normal = (v.y > 0f) ? Vector3.down : Vector3.up;
            else                       normal = (v.z > 0f) ? Vector3.back : Vector3.forward;

            return true;
        }
        
        // Ray (origin o, dir d normalized) vs sphere (center c, radius R). Returns t along ray and hit normal.
        static bool RaySphere(Vector3 o, Vector3 d, Vector3 c, float R, float maxDist, out float t, out Vector3 n)
        {
            t = 0f; n = Vector3.zero;
            Vector3 oc = o - c;
            float b = Vector3.Dot(oc, d);             // note: using form a=1, so 2b in classic quad -> here b
            float cterm = Vector3.Dot(oc, oc) - R*R;
            float disc = b*b - cterm;                 // discriminant of t^2 + 2 b t + c = 0
            if (disc < 0f) return false;

            float sqrtDisc = Mathf.Sqrt(disc);
            float t0 = -b - sqrtDisc;
            float t1 = -b + sqrtDisc;

            // choose smallest non-negative
            float thit = (t0 >= 0f) ? t0 : ((t1 >= 0f) ? t1 : -1f);
            if (thit < 0f || thit > maxDist) return false;

            t = thit;
            Vector3 p = o + d * t;
            n = (p - c).normalized;
            return true;
        }

        public static float EpsilonFor(Vector3 v) => 0.0001f / (v.magnitude + 1e-6f);
    }
}