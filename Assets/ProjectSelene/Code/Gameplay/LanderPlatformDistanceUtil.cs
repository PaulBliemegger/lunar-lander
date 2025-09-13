using System;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class LanderPlatformDistanceUtil : MonoBehaviour
    {
        public bool IsInActivationDistance {private set; get;}
        public float DistanceToPlatform {private set; get;}
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform lander;
        [SerializeField] private Transform platform;
        [SerializeField] private float activationDistance;

        private void Update()
        {
            var distanceToPlatform = Vector3.Distance(platform.position, lander.position);
            DistanceToPlatform = distanceToPlatform;
            if (distanceToPlatform < activationDistance)
            {
                gameManager.OnSecondCameraDistance();
                IsInActivationDistance = true;
            }
            else
            {
                IsInActivationDistance = false;
            }
        }
    }
}