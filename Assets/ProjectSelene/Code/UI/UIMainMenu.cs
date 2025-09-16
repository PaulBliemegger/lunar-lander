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
        
        
        private FloatField _gravitationalPullField;
        private FloatField _massField;
        private FloatField _linearDampingField;
        private IntegerField _maxFuelField;
        private IntegerField _fuelCostField;
        private FloatField _mainThrustFuelFactorField;
        private FloatField _mainThrustField;
        private FloatField _sideThrustField;
        private FloatField _safeLandingSpeedField;
        
        
        private float _gravitationalPull;
        private float _mass;
        private float _linearDamping;
        private int _maxFuel;
        private int _fuelCost;
        private float _mainThrustFuelFactor;
        private float _mainThrust;
        private float _sideThrust;
        private float _safeLandingSpeed;

        private string _activeConfig;

        private void Start()
        {
            ShowMainMenu();
            _activeConfig = "easy";
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
            LoadConfig(_activeConfig);
            uiDocument.visualTreeAsset = configurationMenu;
            var root = uiDocument.rootVisualElement;
            var returnButton = root.Q<Button>("return-main-menu__button");
            returnButton.clicked += ShowMainMenu;
            
            var resetButton = root.Q<Button>("reset__button");
            resetButton.clicked += ResetConfig;
            
            var saveButton = root.Q<Button>("save__button");
            saveButton.clicked += SaveNewConfig;
            
            var easyRadioButton = root.Q<RadioButton>("easy__radio");
            easyRadioButton.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue)
                {
                    LoadConfig(easyRadioButton.text);
                    UpdateFields();
                }
                
            });
            
            var normalRadioButton = root.Q<RadioButton>("normal__radio");
            normalRadioButton.RegisterValueChangedCallback((evt) =>
            {
                
                if (evt.newValue)
                {
                    LoadConfig(normalRadioButton.text);
                    UpdateFields();
                }
            });
            
            var advancedRadioButton = root.Q<RadioButton>("advanced__radio");
            advancedRadioButton.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue)
                {
                    LoadConfig(advancedRadioButton.text);
                    UpdateFields();
                }
            });
            
            _gravitationalPullField = root.Q<FloatField>("gravitational-pull__field");
            _gravitationalPullField.value = _gravitationalPull;
            _gravitationalPullField.RegisterValueChangedCallback((evt) => _gravitationalPull = evt.newValue);
            
            _massField = root.Q<FloatField>("mass__field");
            _massField.value = _mass;
            _massField.RegisterValueChangedCallback((evt) => _mass = evt.newValue);
            
            _maxFuelField = root.Q<IntegerField>("max-tank__field");
            _maxFuelField.value = _maxFuel;
            _maxFuelField.RegisterValueChangedCallback((evt) => _maxFuel = evt.newValue);
            
            _fuelCostField = root.Q<IntegerField>("fuel-cost__field");
            _fuelCostField.value = _fuelCost;
            _fuelCostField.RegisterValueChangedCallback((evt) => _fuelCost = evt.newValue);
            
            _mainThrustFuelFactorField = root.Q<FloatField>("main-thrust-fuel__field");
            _mainThrustFuelFactorField.value = _mainThrustFuelFactor;
            _mainThrustFuelFactorField.RegisterValueChangedCallback((evt) => _mainThrustFuelFactor = evt.newValue);
            
            _mainThrustField = root.Q<FloatField>("main-thrust__field");
            _mainThrustField.value = _mainThrust;
            _mainThrustField.RegisterValueChangedCallback((evt) => _mainThrust = evt.newValue);
            
            _sideThrustField = root.Q<FloatField>("side-thrust__field");
            _sideThrustField.value = _sideThrust;
            _sideThrustField.RegisterValueChangedCallback((evt) => _sideThrust = evt.newValue);
            
            _safeLandingSpeedField = root.Q<FloatField>("safe-landing__field");
            _safeLandingSpeedField.value = _safeLandingSpeed;
            _safeLandingSpeedField.RegisterValueChangedCallback((evt) => _safeLandingSpeed = evt.newValue);
        }

        private void UpdateFields()
        {
            _gravitationalPullField.value = _gravitationalPull;
            _massField.value = _mass;
            _maxFuelField.value = _maxFuel;
            _fuelCostField.value = _fuelCost;
            _mainThrustFuelFactorField.value = _mainThrustFuelFactor;
            _mainThrustField.value = _mainThrust;
            _sideThrustField.value = _sideThrust;
            _safeLandingSpeedField.value = _safeLandingSpeed;
            
        }

        private void LoadConfig(string key)
        {
            var config = configBootstrapper.Config;
            config.LoadConfig(key);
            _gravitationalPull = config.gravitationalPull;
            _mass = config.mass;
            _maxFuel = config.maxFuel;
            _fuelCost = config.fuelCost;
            _mainThrustFuelFactor = config.mainThrustFuelFactor;
            _mainThrust = config.mainThrust;
            _sideThrust = config.sideThrust;
            _safeLandingSpeed = config.safeLandingSpeed;
            _activeConfig = key;
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
            
            config.SaveNewConfig(_activeConfig);
        }

        private void ResetConfig()
        {
            var config = configBootstrapper.Config;
            config.ResetConfig(_activeConfig);
            LoadConfig(_activeConfig);
        }
    }
}