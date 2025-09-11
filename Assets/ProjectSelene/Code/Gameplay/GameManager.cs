using System;
using ProjectSelene.Code.CustomPhysics;
using ProjectSelene.Code.Gameplay.Lander;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject landerModel;
        [SerializeField] private GameObject lander;
        [SerializeField] private Camera cam;
        [SerializeField] private GameObject secondCam;
        private CustomRigidbody _landerRb;
        private CustomGravity _landerGravity;
        private LanderMovement  _landerMovement;

        private void Start()
        {
            _landerRb = lander.GetComponent<CustomRigidbody>();
            _landerGravity = lander.GetComponent<CustomGravity>();
            _landerMovement = lander.GetComponent<LanderMovement>();
        }

        public void OnSafeLanding()
        {
            _landerGravity.enabled = false;
            _landerRb.Velocity = Vector3.zero;
            _landerMovement.enabled = false;
        }

        public void OnCrash()
        {
            cam.transform.SetParent(null);
            lander.SetActive(false);
        }

        public void OnSecondCameraDistance()
        {
            secondCam.SetActive(true);
        }
    }
}