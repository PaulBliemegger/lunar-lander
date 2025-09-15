using System;
using ProjectSelene.Code.CustomPhysics;
using ProjectSelene.Code.Gameplay.Lander;
using ProjectSelene.Code.UI;
using UnityEngine;

namespace ProjectSelene.Code.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject landerModel;
        [SerializeField] private GameObject lander;
        [SerializeField] private Camera cam;
        [SerializeField] private GameObject secondCam;
        [SerializeField] private UiGame ui;
        private CustomRigidbody _landerRb;
        private CustomGravity _landerGravity;
        private LanderMovement  _landerMovement;

        private void Start()
        {
            _landerRb = lander.GetComponent<CustomRigidbody>();
            _landerGravity = lander.GetComponent<CustomGravity>();
            _landerMovement = lander.GetComponent<LanderMovement>();
        }

        public void OnSafeLanding(float speed)
        {
            _landerGravity.enabled = false;
            _landerRb.Velocity = Vector3.zero;
            _landerMovement.enabled = false;
            ui.ShowLandedUI(speed);
        }

        public void OnCrash(string otherName, float speed)
        {
            cam.transform.SetParent(null);
            lander.SetActive(false);
            ui.ShowCrashedUI(otherName, speed);
        }

        public void OnSecondCameraDistance()
        {
            secondCam.SetActive(true);
        }
    }
}