using System;
using UnityEngine;

namespace ProjectSelene.Code
{
    public class GameConfig : MonoBehaviour
    {
        [SerializeField] private bool loadConfigFromFile;
        
        [Header("World")]
        public float gravitationalPull = 100.62f;

        [Header("Lander")]
        public float mass = 1f;
        public float linearDamping = 0.00f;

        [Header("Fuel")]
        public int maxTank = 1000;
        public int fuelCost = 1;

        [Header("Thrusters")]
        public float baseThrust = 40f;
        public float mainThrust = 12f;
        public float sideThrust = 1f;

        [Header("Landing")]
        public float safeLandingSpeed = 5f;

        private void Awake()
        {
            if (loadConfigFromFile)
            {
                ConfigFile.TryLoad(out GameConfigData data);
                GameConfigIO.Apply(data, this);
            }
        }
    }
}