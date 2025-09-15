using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ProjectSelene.Code.UI
{
    public class UiGame : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset gameUI;
        [SerializeField] private VisualTreeAsset pauseMenu;
        [SerializeField] private VisualTreeAsset gameOverUi;
        [SerializeField] private UiUpdateTank uiUpdateTank;
        [SerializeField] private UiUpdateStats uiUpdateStats;

        private bool _isGameUi;
        private DefaultInputActions _inputActions;

        private void Start()
        {
            ShowGameUI();
        }

        private void Awake()
        {
            _inputActions = new DefaultInputActions();
            _inputActions.Player.Pause.performed += context =>
            {
                if (_isGameUi)
                {
                    ShowPauseMenu();
                }
                else
                {
                    ShowGameUI();
                }
            };
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void ShowGameUI()
        {
            Time.timeScale = 1;
            uiDocument.visualTreeAsset = gameUI;
            _isGameUi = true;
            uiUpdateTank.ConnectUI();
            uiUpdateStats.ConnectUI();
        }

        private void ShowPauseMenu()
        {
            _isGameUi = false;
            Time.timeScale = 0;
            uiDocument.visualTreeAsset = pauseMenu;
            var root = uiDocument.rootVisualElement;
            var retryButton = root.Q<Button>("retry__button");
            retryButton.clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            var mainMenuButton = root.Q<Button>("main-menu__button");
            mainMenuButton.clicked += () => SceneManager.LoadScene("MenuMain");
            var quitButton = root.Q<Button>("quit__button");
            quitButton.clicked += Application.Quit;
        }

        public void ShowLandedUI(float speed)
        {
            
        }

        public void ShowCrashedUI(string otherName, float speed)
        {
            uiDocument.visualTreeAsset = gameOverUi;
            var root = uiDocument.rootVisualElement;
            var heading = root.Q<Label>("heading__label");
            heading.text = $"Mission Failed\nCrashed against {otherName.ToUpper()}";
            
            var timerLabel  = root.Q<Label>("timer__label");
            timerLabel.text = $"{DisplayTime()}";
        }
        
        string DisplayTime()
        {
            var totalSeconds = Time.realtimeSinceStartup;
            int minutes = (int)((totalSeconds % 3600) / 60);
            int seconds = (int)(totalSeconds % 60);
            int milliseconds = (int)((totalSeconds % 1f) * 1000);

            return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
    }
}