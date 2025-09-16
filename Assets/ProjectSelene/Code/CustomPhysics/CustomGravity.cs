using UnityEngine;

namespace ProjectSelene.Code.CustomPhysics
{
    public class CustomGravity: MonoBehaviour, IConfigConsumer
    {
        [SerializeField] private Vector3 gravitationalCentre = Vector3.zero;
        [SerializeField] private float gStrength = 1.62f;

        private CustomRigidbody _rb;

        void Awake() {
            _rb = GetComponent<CustomRigidbody>();
        }
            

        void FixedUpdate()
        {
            Vector3 dir = (gravitationalCentre - transform.position).normalized;
            _rb.AddForce(dir * (gStrength * _rb.mass)); // F = m * a
        }

        public void ApplyConfig(GameConfig cfg)
        {
            gStrength = cfg.gravitationalPull;
        }
    }
}