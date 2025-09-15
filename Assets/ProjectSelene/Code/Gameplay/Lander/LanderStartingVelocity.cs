using System;
using ProjectSelene.Code.CustomPhysics;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderStartingVelocity : MonoBehaviour
    {
        [SerializeField] private Vector3 velocity;
        private CustomRigidbody _rb;
        private void Start()
        {
            _rb = GetComponent<CustomRigidbody>();
            _rb.Velocity = velocity;
        }
    }
}