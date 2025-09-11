using System;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class ActivateSecondCamera : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform lander;
        [SerializeField] private Transform platform;
        [SerializeField] private float activationDistance;

        private void Update()
        {
            if (Vector3.Distance(platform.position, lander.position) < activationDistance)
            {
                gameManager.OnSecondCameraDistance();
            }
        }
    }
}