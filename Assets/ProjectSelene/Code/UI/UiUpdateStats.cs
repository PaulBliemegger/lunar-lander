using System;
using ProjectSelene.Code.CustomPhysics;
using ProjectSelene.Code.Gameplay;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectSelene.Code.UI
{
    public class UiUpdateStats : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private LanderPlatformDistanceUtil distanceUtil;
        [SerializeField] private CustomRigidbody landerRb;
        
        private VisualElement _statsContainer;
        private Label _distanceValueVE;
        private Label _speedValueVE;
        private Label _velocityValueVE;
        private bool _autoUpdate;

        public void ConnectUI(bool autoUpdate = true)
        {
            var statsContainer = uiDocument.rootVisualElement.Q<VisualElement>("stats--container");
            var distanceContainer = uiDocument.rootVisualElement.Q<VisualElement>("distance-container");
            var speedContainer = uiDocument.rootVisualElement.Q<VisualElement>("speed-container");
            var velocityContainer = uiDocument.rootVisualElement.Q<VisualElement>("velocity-container");
            _distanceValueVE = distanceContainer.Q<Label>("distance--value");
            _speedValueVE = speedContainer.Query<Label>("speed--value");
            _velocityValueVE = velocityContainer.Q<Label>("velocity--value");
            _autoUpdate = autoUpdate;
        }

        private void UpdateStats(float distance, float speed, Vector3 velocity)
        {
            _distanceValueVE.text = $"{Math.Round(distance, 2)}";
            _speedValueVE.text = $"{Math.Round(speed, 2)}";
            _velocityValueVE.text = $"{velocity}";
        }

        public void UpdateStatsOnCollision()
        {
            UpdateStats(distanceUtil.DistanceToPlatform, landerRb.LastImpactSpeed, landerRb.LastImpactVelocity);
        }

        private void FixedUpdate()
        {
            if (_autoUpdate)
            {
                UpdateStats(distanceUtil.DistanceToPlatform, landerRb.Speed, landerRb.Velocity);
            }
        }
    }
}