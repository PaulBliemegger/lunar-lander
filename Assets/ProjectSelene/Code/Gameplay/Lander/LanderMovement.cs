using System;
using ProjectSelene.Code.CustomPhysics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectSelene.Code.Gameplay.Lander
{
    public class LanderMovement : MonoBehaviour, IConfigConsumer
    {
        [Header("Controls")]
        [SerializeField] private float sideThrust = 5f;
        [SerializeField] private float mainThrust = 40f;
        
        [Header("Fuel")]
        [SerializeField] private int maxFuel = 500;
        [SerializeField] private int fuelCost = 1;
        [SerializeField] private float mainThrustFuelFactor = 3f;
        
        private CustomRigidbody _landerRb;
        private DefaultInputActions _inputActions;
        private AttributeCounter _tank;

        public int CurrentTank => _tank.CurrentValue;
        public int MaxTank => _tank.MaxValue;
        
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
                Vector3 force = new Vector3(input.x, 0f, input.y) * sideThrust;
                _landerRb.AddForce(force);
                _tank.CurrentValue -= fuelCost;
            }

            if (_isThrusting)
            {
                _landerRb.AddForce(transform.up * mainThrust);
                _tank.CurrentValue -= (int)math.floor((float)fuelCost * mainThrustFuelFactor);
            }
        }

        public void ApplyConfig(GameConfig gameConfig)
        {
            sideThrust = gameConfig.sideThrust;
            mainThrust = gameConfig.mainThrust;
            maxFuel = gameConfig.maxFuel;
            fuelCost = gameConfig.fuelCost;
            mainThrustFuelFactor = gameConfig.mainThrustFuelFactor;
        }
    }
}