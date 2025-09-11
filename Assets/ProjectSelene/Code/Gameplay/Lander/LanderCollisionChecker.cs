using ProjectSelene.Code.CustomPhysics;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderCollisionChecker : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        public float maxLandingSpeed = 2f;
        public float maxTiltDegrees = 10f;

        private CustomRigidbody _rb;
        void Awake()
        {
            _rb = GetComponent<CustomRigidbody>();
            _rb.OnCollision += OnBodyCollision;
        }

        void OnDestroy()
        {
            if (_rb != null) _rb.OnCollision -= OnBodyCollision;
        }

        private void OnBodyCollision(Collider other)
        {
            // Use the speed at impact from the RB telemetry
            float s = _rb.LastImpactSpeed;

            // Optional: check tilt vs the last impact normal
            float tilt = Vector3.Angle(transform.up, _rb.LastImpactNormal);
            bool gentle = s <= maxLandingSpeed;
            bool upright = tilt <= maxTiltDegrees;

            if (!other.gameObject.CompareTag("Platform"))
            {
                Debug.Log($"💥 Crash on {other.name.ToUpper()} (speed {s:0.00} m/s, tilt {tilt:0.0}°)");
                gameManager.OnCrash();
                return;
            }

            if (gentle && upright)
            {
                Debug.Log($"✅ Safe landing on {other.name} (speed {s:0.00} m/s)");
                gameManager.OnSafeLanding();
            }
            else
            {
                Debug.Log($"💥 Crash on {other.name} (speed {s:0.00} m/s, tilt {tilt:0.0}°)");
                gameManager.OnCrash();
            }
        }
    }
}