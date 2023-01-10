using UnityEngine;
using ZestCore.Utility;

namespace ZestGames
{
    public class UpgradeCanvas : MonoBehaviour
    {
        private CustomButton _closeButton, _emptySpaceButton;
        public static bool IsOpen { get; private set; }

        #region ANIMATION
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private readonly int _closeID = Animator.StringToHash("Close");
        #endregion

        [Header("-- STAMINA SETUP --")]
        [SerializeField] private UpgradeCanvasItem _movementSpeed;
        [SerializeField] private UpgradeCanvasItem _moneyValue;
        [SerializeField] private UpgradeCanvasItem _digSpeed;

        public void Init(UiManager uiManager)
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();

                _movementSpeed.Init(this, UpgradeCanvasItem.UpgradeItemType.MovementSpeed);
                _moneyValue.Init(this, UpgradeCanvasItem.UpgradeItemType.MoneyValue);
                _digSpeed.Init(this, UpgradeCanvasItem.UpgradeItemType.DigSpeed);
            }

            Delayer.DoActionAfterDelay(this, 0.5f, UpdateTexts);

            IsOpen = false;

            PlayerUpgradeEvents.OnUpdateUpgradeTexts += UpdateTexts;

            PlayerUpgradeEvents.OnOpenCanvas += EnableCanvas;
            PlayerUpgradeEvents.OnCloseCanvas += DisableCanvas;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpdateUpgradeTexts -= UpdateTexts;

            PlayerUpgradeEvents.OnOpenCanvas -= EnableCanvas;
            PlayerUpgradeEvents.OnCloseCanvas -= DisableCanvas;
        }

        #region UPDATERS
        private void UpdateTexts()
        {
            _movementSpeed.LevelText.text = $"Level {DataManager.MovementSpeedLevel}";
            _movementSpeed.CostText.text = DataManager.MovementSpeedCost.ToString();
            _movementSpeed.CheckForMoneySufficiency();

            _moneyValue.LevelText.text = $"Level {DataManager.MoneyValueLevel}";
            _moneyValue.CostText.text = DataManager.MoneyValueCost.ToString();
            _moneyValue.CheckForMoneySufficiency();

            _digSpeed.LevelText.text = $"Level {DataManager.DigSpeedLevel}";
            _digSpeed.CostText.text = DataManager.DigSpeedCost.ToString();
            _digSpeed.CheckForMoneySufficiency();
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void CloseCanvas()
        {
            PlayerUpgradeEvents.OnCloseCanvas?.Invoke();
            PlayerEvents.OnClosedUpgradeCanvas?.Invoke();
        }
        #endregion

        #region ANIMATOR FUNCTIONS
        private void EnableCanvas()
        {
            _animator.SetTrigger(_openID);
            IsOpen = true;
        }
        private void DisableCanvas()
        {
            if (!IsOpen) return;

            AudioManager.PlayAudio(Enums.AudioType.UpgradeMenu);
            
            _animator.SetTrigger(_closeID);
            IsOpen = false;
        }
        #endregion
    }
}
