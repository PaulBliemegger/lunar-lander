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
            OnLand();
            _landerGravity.enabled = false;
            _landerRb.Velocity = Vector3.zero;
            _landerMovement.enabled = false;
        }

        public void OnCrash()
        {
            OnLand();
            cam.transform.SetParent(null);
            lander.SetActive(false);
            //landerModel.SetActive(false);
        }

        public void OnLand()
        {
            //_landerMovement.enabled = false;
            //cam.transform.SetParent(null);
            //lander.SetActive(false);
        }
    }
}