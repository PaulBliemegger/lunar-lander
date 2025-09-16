using System;
using UnityEngine;

namespace ProjectSelene.Code
{
    public class GameConfig : MonoBehaviour
    {
        [SerializeField] private bool loadConfigFromFile;
        [SerializeField] private string configFileName;
        
        [Header("Gravity")]
        public float gravitationalPull = 1.62f;

        [Header("Lander RB")]
        public float mass = 1f;
        public float linearDamping = 0.00f;

        [Header("Lander Fuel")]
        public int maxFuel = 1000;
        public int fuelCost = 1;
        public float mainThrustFuelFactor = 3f;

        [Header("Lander Thrusters")]
        public float mainThrust = 40f;
        public float sideThrust = 5f;

        [Header("Success - Landing")]
        public float safeLandingSpeed = 5f;

        private void Awake()
        {
            if (loadConfigFromFile)
            {
                LoadConfig(configFileName);
            }
        }

        public void SaveNewConfig(string key)
        {
            ConfigFile.SaveOverride(key, GameConfigIO.FromAsset(this));
        }

        public void LoadConfig(string key)
        {
            GameConfigLoader.TryLoadEffective(key, out GameConfigData data);
            GameConfigIO.Apply(data, this);
        }

        public void ResetConfig(string key)
        {
            ConfigFile.DeleteOverride(key);
            LoadConfig(key);
        }
    }
}