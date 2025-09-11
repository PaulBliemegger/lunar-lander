using System;
using System.Collections.Generic;
using UnityEngine;
namespace ProjectSelene.Code.CustomPhysics
{

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Collider))] // BoxCollider or SphereCollider for the lander
public class CustomRigidbody : MonoBehaviour
{
    [Header("Motion")]
    public float mass = 1f;
    public float linearDamping = 0.05f;
    public Vector3 Velocity { get; set; }
    public float Speed => Velocity.magnitude;

    [Header("Collision")]
    public bool useCollisions = true;
    public LayerMask collisionLayers = ~0;
    public float skinWidth = 0.01f;
    public int maxSweepSteps = 3;

    // Telemetry
    public bool IsGrounded { get; private set; }
    public float LastImpactSpeed { get; private set; }
    public Vector3 LastImpactNormal { get; private set; } = Vector3.up;

    // Event: raised when this body first contacts another collider in a step
    public event Action<Collider> OnCollision;

    private Vector3 _forceAccum;
    private Collider _self;
    private readonly List<Collider> _obstacles = new();

    void Awake()
    {
        _self = GetComponent<Collider>();
        RebuildObstacleList();
    }

    /// Call this if you add/remove obstacles at runtime (or change layers)
    public void RebuildObstacleList()
    {
        _obstacles.Clear();
        var all = FindObjectsOfType<Collider>(includeInactive: false);
        foreach (var c in all)
        {
            if (c == _self) continue;
            if (!c.enabled || !c.gameObject.activeInHierarchy) continue;

            // Only accept BoxCollider or SphereCollider as requested
            if (!(c is BoxCollider || c is SphereCollider)) continue;

            if (((1 << c.gameObject.layer) & collisionLayers.value) == 0) continue;
            _obstacles.Add(c);
        }
    }

    public void AddForce(Vector3 force) => _forceAccum += force;

    public void AddImpulse(Vector3 impulse)
    {
        if (mass > 0f) Velocity += impulse / mass;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (dt <= 0f || mass <= 0f) { _forceAccum = Vector3.zero; return; }

        // a = F/m; v += a*dt
        Vector3 acc = _forceAccum / mass;
        Velocity += acc * dt;

        // damping
        if (linearDamping > 0f)
            Velocity *= Mathf.Clamp01(1f - linearDamping * dt);

        // integrate
        if (useCollisions) MoveWithSweptAABB(dt);
        else transform.position += Velocity * dt;

        _forceAccum = Vector3.zero;
    }

    // ---------- Collision (custom; box + sphere obstacles) ----------

    // World-space AABB for our own collider, used for Minkowski expansion (point vs expanded AABB)
    Bounds GetSelfAABB()
    {
        if (_self is BoxCollider sb) return CustomColliderUtility.GetAABB(sb);
        if (_self is SphereCollider ss) return CustomColliderUtility.GetAABB(ss);
        // Fallback: empty
        return new Bounds(transform.position, Vector3.zero);
    }



    void MoveWithSweptAABB(float dt)
    {
        IsGrounded = false;

        Vector3 remaining = Velocity * dt;
        Bounds selfAABB = GetSelfAABB();

        for (int step = 0; step < maxSweepSteps; step++)
        {
            if (remaining.sqrMagnitude < 1e-10f) break;

            Vector3 start = selfAABB.center;
            Vector3 v = remaining;

            float bestTOI = 1f;
            Vector3 bestNormal = Vector3.zero;
            Collider bestHit = null;

            foreach (var obs in _obstacles)
            {
                if (!obs || !obs.enabled || !obs.gameObject.activeInHierarchy) continue;

                Bounds target = (obs is BoxCollider ob) ? CustomColliderUtility.GetAABB(ob) : CustomColliderUtility.GetAABB((SphereCollider)obs);

                // Minkowski sum: expand obstacle AABB by our AABB size (point-vs-box cast)
                Bounds expanded = new Bounds(target.center, target.size + selfAABB.size);

                if (RayAABB(start, v, expanded, out float tEntry, out Vector3 n))
                {
                    if (tEntry < bestTOI && tEntry >= 0f)
                    {
                        bestTOI = tEntry;
                        bestNormal = n;
                        bestHit = obs;
                    }
                }
            }

            if (!bestHit)
            {
                // free move
                transform.position += remaining;
                break;
            }

            // Move to just before impact
            Vector3 hitMove = v * Mathf.Max(0f, bestTOI - EpsilonFor(v));
            transform.position += hitMove;

            // Telemetry
            LastImpactSpeed = Velocity.magnitude;
            LastImpactNormal = bestNormal;

            // Slide: remove velocity into normal
            float vn = Vector3.Dot(Velocity, bestNormal);
            if (vn < 0f) Velocity -= vn * bestNormal;

            // Ground check
            if (Vector3.Angle(bestNormal, Vector3.up) < 60f) IsGrounded = true;

            // Raise event (other collider only, as requested)
            OnCollision?.Invoke(bestHit);

            // Continue with leftover fraction
            float remainingFrac = Mathf.Clamp01(1f - bestTOI);
            remaining = Velocity * (dt * remainingFrac);

            // Nudge out a bit
            transform.position += bestNormal * skinWidth;

            // Update our AABB center after moving
            selfAABB = GetSelfAABB();
        }
    }

    // Ray (p -> p+v) vs AABB (slab method). Returns hit within [0,1]
    static bool RayAABB(Vector3 p, Vector3 v, Bounds b, out float tEntry, out Vector3 normal)
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

    static float EpsilonFor(Vector3 v) => 0.0001f / (v.magnitude + 1e-6f);

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Velocity);
    }
#endif
}



}