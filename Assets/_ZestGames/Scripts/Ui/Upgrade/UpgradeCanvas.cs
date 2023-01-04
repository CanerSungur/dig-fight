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
        [SerializeField] private UpgradeCanvasItem moneyValue;
        [SerializeField] private UpgradeCanvasItem pickaxeSpeed;
        [SerializeField] private UpgradeCanvasItem pickaxeDurability;
        [SerializeField] private UpgradeCanvasItem pickaxePower;

        public void Init(UiManager uiManager)
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();

                moneyValue.Init(this, UpgradeCanvasItem.UpgradeItemType.MoneyValue);
                pickaxeSpeed.Init(this, UpgradeCanvasItem.UpgradeItemType.PickaxeSpeed);
                pickaxeDurability.Init(this, UpgradeCanvasItem.UpgradeItemType.PickaxeDurability);
                pickaxePower.Init(this, UpgradeCanvasItem.UpgradeItemType.PickaxePower);
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
            moneyValue.LevelText.text = $"Level {DataManager.MoneyValueLevel}";
            moneyValue.CostText.text = DataManager.MoneyValueCost.ToString();
            moneyValue.CheckForMoneySufficiency();

            pickaxeSpeed.LevelText.text = $"Level {DataManager.PickaxeSpeedLevel}";
            pickaxeSpeed.CostText.text = DataManager.PickaxeSpeedCost.ToString();
            pickaxeSpeed.CheckForMoneySufficiency();

            pickaxeDurability.LevelText.text = $"Level {DataManager.PickaxeDurabilityLevel}";
            pickaxeDurability.CostText.text = DataManager.PickaxeDurabilityCost.ToString();
            pickaxeDurability.CheckForMoneySufficiency();

            pickaxePower.LevelText.text = $"Level {DataManager.PickaxePowerLevel}";
            pickaxePower.CostText.text = DataManager.PickaxePowerCost.ToString();
            pickaxePower.CheckForMoneySufficiency();
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
