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
    }
}