using ProjectSelene.Code.Gameplay.Lander;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectSelene.Code.UI
{
    public class UiUpdateTank : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private LanderMovement landerMovement;
        
        private ProgressBar _tankProgressBar;

        private void Start()
        {
            _tankProgressBar = uiDocument.rootVisualElement.Q<ProgressBar>("tank-progress-bar");
            _tankProgressBar.highValue = landerMovement.MaxTank;
            _tankProgressBar.value = landerMovement.CurrentTank;
        }

        private void LateUpdate()
        {
           _tankProgressBar.value = landerMovement.CurrentTank;
        }
    }
}