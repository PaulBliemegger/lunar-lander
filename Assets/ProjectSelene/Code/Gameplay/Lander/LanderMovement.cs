using System;
using ProjectSelene.Code.CustomPhysics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderMovement : MonoBehaviour
    {
        [SerializeField] private float stabilisationPower = 5f;
        [SerializeField] private float thrustPower = 5f;
        
        private CustomRigidbody _landerRb;
        private DefaultInputActions _inputActions;
        
        private Vector2 _inputVector;
        private bool _isThrusting;
        private void Awake()
        {
            _landerRb = GetComponent<CustomRigidbody>();
            _inputActions = new DefaultInputActions();
            _inputActions.Player.Thrust.performed += (_ => _isThrusting = true);
            _inputActions.Player.Thrust.canceled += (_ => _isThrusting = false);
        }
        
        void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }
        

        private void FixedUpdate()
        {
            Vector2 input = _inputActions.Player.Movement.ReadValue<Vector2>();
            
            if (input.sqrMagnitude > 0f)
            {
                Vector3 force = new Vector3(input.x, 0f, input.y) * stabilisationPower;
                _landerRb.AddForce(force);
            }

            if (_isThrusting)
            {
                _landerRb.AddForce(transform.up * thrustPower);
            }
        }
    }
}