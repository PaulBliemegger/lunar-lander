using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProjectSelene.Code.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset mainMenu;
        [SerializeField] private VisualTreeAsset mapsMenu;
        [SerializeField] private VisualTreeAsset configurationMenu;
        [SerializeField] private ConfigBootstrapper configBootstrapper;
        
        private float _gravitationalPull;
        private float _mass;
        private float _linearDamping;
        private int _maxFuel;
        private int _fuelCost;
        private float _mainThrustFuelFactor;
        private float _mainThrust;
        private float _sideThrust;
        private float _safeLandingSpeed;

        private void Start()
        {
            ShowMainMenu();
        }
        
        private void ShowMainMenu()
        {
            uiDocument.visualTreeAsset = mainMenu;
            
            var root = uiDocument.rootVisualElement;
            var mapsButton = root.Q<Button>("maps-button");
            mapsButton.clicked += ShowMaps ;
            
            var configurationButton = root.Q<Button>("configuration-button");
            configurationButton.clicked += ShowConfiguration;
            
            var quitButton = root.Q<Button>("quit-button");
            quitButton.clicked += () =>
            {
                Application.Quit();
            };
        }

        private void ShowMaps()
        {
            uiDocument.visualTreeAsset = mapsMenu;
            var root = uiDocument.rootVisualElement;
            var returnButton = root.Q<Button>("return-main-menu__button");
            returnButton.clicked += ShowMainMenu;
            
            var easyButton = root.Q<Button>("easy-map__button");
            easyButton.clicked += () =>
            {
                SceneManager.LoadScene("EasyMap");
            };
            
            var normalButton = root.Q<Button>("normal-map__button");
            normalButton.clicked += () =>
            {
                SceneManager.LoadScene("NormalMap");
            };
            
            var advancedButton = root.Q<Button>("advanced-map__button");
            advancedButton.clicked += () =>
            {
                SceneManager.LoadScene("AdvancedMap");
            };
        }

        private void ShowConfiguration()
        {
            LoadConfig();
            uiDocument.visualTreeAsset = configurationMenu;
            var root = uiDocument.rootVisualElement;
            var returnButton = root.Q<Button>("return-main-menu__button");
            returnButton.clicked += ShowMainMenu;
            
            var gravitationalPull = root.Q<FloatField>("gravitational-pull__field");
            gravitationalPull.value = _gravitationalPull;
            gravitationalPull.RegisterValueChangedCallback((evt) => _gravitationalPull = evt.newValue);
            
            var mass = root.Q<FloatField>("mass__field");
            mass.value = _mass;
            mass.RegisterValueChangedCallback((evt) => _mass = evt.newValue);
            
            var maxFuel = root.Q<IntegerField>("max-tank__field");
            maxFuel.value = _maxFuel;
            maxFuel.RegisterValueChangedCallback((evt) => _maxFuel = evt.newValue);
            
            var fuelCost = root.Q<IntegerField>("fuel-cost__field");
            fuelCost.value = _fuelCost;
            fuelCost.RegisterValueChangedCallback((evt) => _fuelCost = evt.newValue);
            
            var mainThrustFuelFactor = root.Q<FloatField>("main-thrust-fuel__field");
            mainThrustFuelFactor.value = _mainThrustFuelFactor;
            mainThrustFuelFactor.RegisterValueChangedCallback((evt) => _mainThrustFuelFactor = evt.newValue);
            
            var mainThrust = root.Q<FloatField>("main-thrust__field");
            mainThrust.value = _mainThrust;
            mainThrust.RegisterValueChangedCallback((evt) => _mainThrust = evt.newValue);
            
            var sideThrust = root.Q<FloatField>("side-thrust__field");
            sideThrust.value = _sideThrust;
            sideThrust.RegisterValueChangedCallback((evt) => _sideThrust = evt.newValue);
            
            var safeLandingSpeed = root.Q<FloatField>("safe-landing__field");
            safeLandingSpeed.value = _safeLandingSpeed;
            safeLandingSpeed.RegisterValueChangedCallback((evt) => _safeLandingSpeed = evt.newValue);
        }

        private void LoadConfig()
        {
            var config = configBootstrapper.Config;
            _gravitationalPull = config.gravitationalPull;
            _mass = config.mass;
            _maxFuel = config.maxFuel;
            _fuelCost = config.fuelCost;
            _mainThrustFuelFactor = config.mainThrustFuelFactor;
            _mainThrust = config.mainThrust;
            _sideThrust = config.sideThrust;
            _safeLandingSpeed = config.safeLandingSpeed;
        }

        private void SaveNewConfig()
        {
            var config = configBootstrapper.Config;
            config.gravitationalPull = _gravitationalPull;
            config.mass = _mass;
            config.maxFuel = _maxFuel;
            config.fuelCost = _fuelCost;
            config.mainThrustFuelFactor = _mainThrustFuelFactor;
            config.mainThrust = _mainThrust;
            config.sideThrust = _sideThrust;
            config.safeLandingSpeed = _safeLandingSpeed;
        }
        
        
    }
}