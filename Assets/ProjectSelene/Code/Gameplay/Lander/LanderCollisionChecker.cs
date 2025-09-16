using ProjectSelene.Code.CustomPhysics;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderCollisionChecker : MonoBehaviour, IConfigConsumer
    {
        [SerializeField] private GameManager gameManager;
        public float safeLandingSpeed = 5f;

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
            float s = _rb.LastImpactSpeed;
            bool gentle = s <= safeLandingSpeed;

            if (!other.gameObject.CompareTag("Platform"))
            {
                gameManager.OnCrash(other.name, s);
                return;
            }

            if (gentle)
            {
                gameManager.OnSafeLanding(s);
            }
            else
            {
                gameManager.OnCrash(other.name, s);
            }
        }

        public void ApplyConfig(GameConfig gameConfig)
        {
            safeLandingSpeed = gameConfig.safeLandingSpeed;
        }
    }
}