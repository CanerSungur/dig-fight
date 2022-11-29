using UnityEngine;
using ZestCore.Utility;

namespace ZestGames
{
    public class UpgradeCanvas : MonoBehaviour
    {
        public enum Type { Idle, Incremental }
        [SerializeField] private Type _currentType;
        public Type CurrentType => _currentType;

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

        public void Init(UiManager uiManager)
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
                if (_currentType == Type.Idle)
                {
                    _closeButton = transform.GetChild(0).GetChild(0).GetComponent<CustomButton>();
                    _emptySpaceButton = transform.GetChild(1).GetComponent<CustomButton>();
                }

                moneyValue.Init(this);
                pickaxeSpeed.Init(this);
                pickaxeDurability.Init(this);
            }

            Delayer.DoActionAfterDelay(this, 0.5f, UpdateTexts);

            IsOpen = false;

            if (_currentType == Type.Idle)
            {
                _closeButton.onClick.AddListener(CloseCanvasClicked);
                _emptySpaceButton.onClick.AddListener(CloseCanvasClicked);
            }

            moneyValue.Button.onClick.AddListener(MoneyValueUpgradeClicked);
            pickaxeSpeed.Button.onClick.AddListener(PickaxeSpeedUpgradeClicked);
            pickaxeDurability.Button.onClick.AddListener(PickaxeDurabilityUpgradeClicked);

            PlayerUpgradeEvents.OnUpdateUpgradeTexts += UpdateTexts;

            PlayerUpgradeEvents.OnOpenCanvas += EnableCanvas;
            PlayerUpgradeEvents.OnCloseCanvas += DisableCanvas;
        }

        private void OnDisable()
        {
            if (_currentType == Type.Idle)
            {
                _closeButton.onClick.RemoveListener(CloseCanvasClicked);
                _emptySpaceButton.onClick.RemoveListener(CloseCanvasClicked);
            }

            moneyValue.Button.onClick.RemoveListener(MoneyValueUpgradeClicked);
            pickaxeSpeed.Button.onClick.RemoveListener(PickaxeSpeedUpgradeClicked);
            pickaxeDurability.Button.onClick.RemoveListener(PickaxeDurabilityUpgradeClicked);

            PlayerUpgradeEvents.OnUpdateUpgradeTexts -= UpdateTexts;

            PlayerUpgradeEvents.OnOpenCanvas -= EnableCanvas;
            PlayerUpgradeEvents.OnCloseCanvas -= DisableCanvas;
        }

        #region UPDATERS
        private void UpdateTexts()
        {
            #region LIMITED MOVEMENT SPEED
            //if (DataManager.MovementSpeedLevel >= 13)
            //{
            //    movementSpeed.Button.gameObject.SetActive(false);
            //    movementSpeed.LevelText.text = "MAX LEVEL!";
            //}
            //else
            //{
            //    movementSpeed.Button.gameObject.SetActive(true);
            //    movementSpeed.LevelText.text = $"Level {DataManager.MovementSpeedLevel}";
            //    movementSpeed.CostText.text = DataManager.MovementSpeedCost.ToString();
            //}
            #endregion

            moneyValue.LevelText.text = $"Level {DataManager.MoneyValueLevel}";
            moneyValue.CostText.text = DataManager.MoneyValueCost.ToString();

            pickaxeSpeed.LevelText.text = $"Level {DataManager.PickaxeSpeedLevel}";
            pickaxeSpeed.CostText.text = DataManager.PickaxeSpeedCost.ToString();

            pickaxeDurability.LevelText.text = $"Level {DataManager.PickaxeDurabilityLevel}";
            pickaxeDurability.CostText.text = DataManager.PickaxeDurabilityCost.ToString();

            CheckForMoneySufficiency();
        }

        private void CheckForMoneySufficiency()
        {
            moneyValue.Button.interactable = DataManager.TotalMoney >= DataManager.MoneyValueCost;
            pickaxeSpeed.Button.interactable = DataManager.TotalMoney >= DataManager.PickaxeSpeedCost;
            pickaxeDurability.Button.interactable = DataManager.TotalMoney >= DataManager.PickaxeDurabilityCost;
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void CloseCanvas()
        {
            PlayerUpgradeEvents.OnCloseCanvas?.Invoke();
            PlayerEvents.OnClosedUpgradeCanvas?.Invoke();
        }
        private void UpgradeMoneyValue() => PlayerUpgradeEvents.OnUpgradeMoneyValue?.Invoke();
        private void UpgradePickaxeSpeed() => PlayerUpgradeEvents.OnUpgradePickaxeSpeed?.Invoke();
        private void UpgradePickaxeDurability() => PlayerUpgradeEvents.OnUpgradePickaxeDurability?.Invoke();
        #endregion

        #region CLICK TRIGGER FUNCTIONS
        private void CloseCanvasClicked()
        {
            if (_currentType == Type.Idle)
            {
                _closeButton.interactable = _emptySpaceButton.interactable = false;
                _closeButton.TriggerClick(CloseCanvas);
            }
        }
        private void MoneyValueUpgradeClicked() => moneyValue.Button.TriggerClick(UpgradeMoneyValue, moneyValue.ShakeLevelImage);
        private void PickaxeSpeedUpgradeClicked() => pickaxeSpeed.Button.TriggerClick(UpgradePickaxeSpeed, pickaxeSpeed.ShakeLevelImage);
        private void PickaxeDurabilityUpgradeClicked() => pickaxeDurability.Button.TriggerClick(UpgradePickaxeDurability, pickaxeDurability.ShakeLevelImage);
        #endregion

        #region ANIMATOR FUNCTIONS
        private void EnableCanvas()
        {
            //AudioManager.PlayAudio(Enums.AudioType.UpgradeMenu);
            
            if (_currentType == Type.Idle)
                _closeButton.interactable = _emptySpaceButton.interactable = true;
            
            _animator.SetTrigger(_openID);
            IsOpen = true;

            CheckForMoneySufficiency();
        }
        private void DisableCanvas()
        {
            AudioManager.PlayAudio(Enums.AudioType.UpgradeMenu);
            
            if (_currentType == Type.Idle)
                _closeButton.interactable = _emptySpaceButton.interactable = false;
            
            _animator.SetTrigger(_closeID);
            IsOpen = false;
        }
        #endregion
    }
}
