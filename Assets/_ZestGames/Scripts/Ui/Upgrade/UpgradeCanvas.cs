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
        [SerializeField] private UpgradeCanvasItem movementSpeed;
        [SerializeField] private UpgradeCanvasItem moneyValue;

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

                movementSpeed.Init(this);
                moneyValue.Init(this);
            }

            Delayer.DoActionAfterDelay(this, 0.5f, UpdateTexts);

            IsOpen = false;

            if (_currentType == Type.Idle)
            {
                _closeButton.onClick.AddListener(CloseCanvasClicked);
                _emptySpaceButton.onClick.AddListener(CloseCanvasClicked);
            }

            movementSpeed.Button.onClick.AddListener(MovementSpeedUpgradeClicked);
            moneyValue.Button.onClick.AddListener(MoneyValueUpgradeClicked);

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

            movementSpeed.Button.onClick.RemoveListener(MovementSpeedUpgradeClicked);
            moneyValue.Button.onClick.RemoveListener(MoneyValueUpgradeClicked);

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

            if (_currentType == Type.Idle)
                movementSpeed.LevelText.text = $"Level {DataManager.MovementSpeedLevel}";
            else
                movementSpeed.LevelText.text = DataManager.MovementSpeedLevel.ToString();
            movementSpeed.CostText.text = DataManager.MovementSpeedCost.ToString();

            if (_currentType == Type.Idle)
                moneyValue.LevelText.text = $"Level {DataManager.MoneyValueLevel}";
            else
                moneyValue.LevelText.text = DataManager.MoneyValueLevel.ToString();
            moneyValue.CostText.text = DataManager.MoneyValueCost.ToString();

            CheckForMoneySufficiency();
        }

        private void CheckForMoneySufficiency()
        {
            movementSpeed.Button.interactable = DataManager.TotalMoney >= DataManager.MovementSpeedCost;
            moneyValue.Button.interactable = DataManager.TotalMoney >= DataManager.MoneyValueCost;
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void CloseCanvas()
        {
            PlayerUpgradeEvents.OnCloseCanvas?.Invoke();
            PlayerEvents.OnClosedUpgradeCanvas?.Invoke();
        }
        private void UpgradeMovementSpeed() => PlayerUpgradeEvents.OnUpgradeMovementSpeed?.Invoke();
        private void UpgradeMoneyValue() => PlayerUpgradeEvents.OnUpgradeMoneyValue?.Invoke();
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
        private void MovementSpeedUpgradeClicked() => movementSpeed.Button.TriggerClick(UpgradeMovementSpeed);
        private void MoneyValueUpgradeClicked() => moneyValue.Button.TriggerClick(UpgradeMoneyValue);
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
