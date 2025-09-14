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
        }

        private void ShowConfiguration()
        {
            uiDocument.visualTreeAsset = configurationMenu;
            var root = uiDocument.rootVisualElement;
            var returnButton = root.Q<Button>("return-main-menu__button");
            returnButton.clicked += ShowMainMenu;
        }
        
    }
}