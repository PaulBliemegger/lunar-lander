using System;
using ProjectSelene.Code.CustomPhysics;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProjectSelene.Code
{
    public class CustomGravity: MonoBehaviour
    {
        [SerializeField] private Vector3 gravitationalCentre = Vector3.zero;
        [SerializeField] private float gStrength = 1.62f;

        private CustomRigidbody _rb;

        void Awake() => _rb = GetComponent<CustomRigidbody>();

        void FixedUpdate()
        {
            Vector3 dir = (gravitationalCentre - transform.position).normalized;
            _rb.AddForce(dir * (gStrength * _rb.mass)); // F = m * a
        }
    }
}