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

        private void Start()
        {
            var statsContainer = uiDocument.rootVisualElement.Q<VisualElement>("stats--container");
            var distanceContainer = uiDocument.rootVisualElement.Q<VisualElement>("distance-container");
            var speedContainer = uiDocument.rootVisualElement.Q<VisualElement>("speed-container");
            var velocityContainer = uiDocument.rootVisualElement.Q<VisualElement>("velocity-container");
            _distanceValueVE = distanceContainer.Q<Label>("distance--value");
            _speedValueVE = speedContainer.Query<Label>("speed--value");
            _velocityValueVE = velocityContainer.Q<Label>("velocity--value");
        }

        private void FixedUpdate()
        {
            _distanceValueVE.text = $"{Math.Round(distanceUtil.DistanceToPlatform, 2)}";
            _speedValueVE.text = $"{Math.Round(landerRb.Speed, 2)}";
            _velocityValueVE.text = $"{landerRb.Velocity}";
        }
    }
}