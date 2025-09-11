using System;
using ProjectSelene.Code.CustomPhysics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderMovement : MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField] private float stabilisationPower = 5f;
        [SerializeField] private float thrustPower = 5f;
        
        [Header("Fuel")]
        [SerializeField] private int maxFuel = 500;
        [SerializeField] private int fuelCost = 1;
        [SerializeField] private float fuelThrustingFactor = 3;
        
        private CustomRigidbody _landerRb;
        private DefaultInputActions _inputActions;
        private AttributeCounter _tank;
        
        private Vector2 _inputVector;
        private bool _isThrusting;
        private void Awake()
        {
            _tank = new (maxFuel);
            
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

            if (_tank.CurrentValue <= 0)
            {
                return;
            }
            
            if (input.sqrMagnitude > 0f)
            {
                Vector3 force = new Vector3(input.x, 0f, input.y) * stabilisationPower;
                _landerRb.AddForce(force);
                _tank.CurrentValue -= fuelCost;
                //Debug.Log(_tank.CurrentValue);
            }

            if (_isThrusting)
            {
                _landerRb.AddForce(transform.up * thrustPower);
                _tank.CurrentValue -= (int)math.floor((float)fuelCost * fuelThrustingFactor);
                //Debug.Log(_tank.CurrentValue);
            }
        }
    }
}