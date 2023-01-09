using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using DigFight;
using UnityEngine.UI;

namespace ZestGames
{
    public class UpgradeCanvasItem : MonoBehaviour
    {
        public enum UpgradeItemType { MoneyValue, PickaxeSpeed, PickaxePower, PickaxeDurability }
        private UpgradeItemType _upgradeItemType;

        #region COMPONENTS
        private UpgradeCanvas _upgradeCanvas;
        private UpgradeItemAdHandler _adHandler;
        #endregion

        #region PRIVATES
        private CustomButton _upgradeButton, _optionalAdButton, _forcedAdButton;
        private int _currentLevel = 1;
        #endregion

        #region PROPERTIES
        public TextMeshProUGUI LevelText { get; private set; }
        public TextMeshProUGUI CostText { get; private set; }
        public CustomButton ActiveUpgradeButton { get; private set; }
        #endregion

        #region LEVEL SHAKE SEQUENCE
        private Sequence _shakeLevelImageSequence;
        private Guid _shakeLevelImageSequenceID;
        private const float SHAKE_DURATION = 0.5f;
        private Transform _levelImage;
        #endregion

        public void Init(UpgradeCanvas upgradeCanvas, UpgradeItemType upgradeItemType)
        {
            if (_upgradeCanvas == null)
            {
                _upgradeCanvas = upgradeCanvas;
                _upgradeItemType = upgradeItemType;
                _adHandler = GetComponent<UpgradeItemAdHandler>();
                _adHandler.Init(this);
            }

            LevelText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            CostText = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
            _upgradeButton = transform.GetChild(2).GetComponent<CustomButton>();
            _forcedAdButton = transform.GetChild(3).GetComponent<CustomButton>();
            _optionalAdButton = transform.GetChild(4).GetComponent<CustomButton>();

            _upgradeButton.onClick.AddListener(() => UpgradeButtonClicked(_upgradeButton));
            _forcedAdButton.onClick.AddListener(() => UpgradeButtonClicked(_forcedAdButton));

            GetCurrentLevel();
            _optionalAdButton.gameObject.SetActive(false);
            CheckForForcedUpgrade();
        }

        private void OnDisable()
        {
            if (_upgradeCanvas == null) return;

            _upgradeButton.onClick.RemoveListener(() => UpgradeButtonClicked(_upgradeButton));
            _forcedAdButton.onClick.RemoveListener(() => UpgradeButtonClicked(_forcedAdButton));
        }

        #region EVENT HANDLER FUNCTIONS
        
        private void UpgradeButtonClicked(CustomButton customButton)
        {
            if (customButton == _upgradeButton)
                ActiveUpgradeButton.TriggerClick(Upgrade, ShakeLevelImage);
            else if (customButton == _forcedAdButton)
                ActiveUpgradeButton.TriggerClick(() => _adHandler.OpenRewardedAd(RewardedAdUpgrade));
        }
        #endregion

        #region CHECK FUNCTIONS
        private void CheckForForcedUpgrade()
        {
            //Debug.Log(_currentLevel);
            _adHandler.CheckForAd(_currentLevel);
            CheckUpgradeButtonActivation();
        }
        private void CheckUpgradeButtonActivation()
        {
            if (_adHandler.UpgradeIsEnabled)
            {
                _upgradeButton.gameObject.SetActive(true);
                _forcedAdButton.gameObject.SetActive(false);
                ActiveUpgradeButton = _upgradeButton;

                CheckForMoneySufficiency();
            }
            else
            {
                _upgradeButton.gameObject.SetActive(false);
                _forcedAdButton.gameObject.SetActive(true);
                ActiveUpgradeButton = _forcedAdButton;
            }
        }
        #endregion

        #region HELPERS
        private void ShakeLevelImage() => StartShakeLevelImageSequence();
        private void GetCurrentLevel()
        {
            if (_upgradeItemType == UpgradeItemType.MoneyValue)
                _currentLevel = DataManager.MoneyValueLevel;
            else if (_upgradeItemType == UpgradeItemType.PickaxeDurability)
                _currentLevel = DataManager.PickaxeDurabilityLevel;
            else if (_upgradeItemType == UpgradeItemType.PickaxePower)
                _currentLevel = DataManager.PickaxePowerLevel;
            else if (_upgradeItemType == UpgradeItemType.PickaxeSpeed)
                _currentLevel = DataManager.PickaxeSpeedLevel;
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void Upgrade()
        {
            if (_upgradeItemType == UpgradeItemType.MoneyValue)
            {
                PlayerUpgradeEvents.OnUpgradeMoneyValue?.Invoke(false);
                _currentLevel = DataManager.MoneyValueLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxeDurability)
            {
                PlayerUpgradeEvents.OnUpgradePickaxeDurability?.Invoke(false);
                _currentLevel = DataManager.PickaxeDurabilityLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxePower)
            {
                PlayerUpgradeEvents.OnUpgradePickaxePower?.Invoke(false);
                _currentLevel = DataManager.PickaxePowerLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxeSpeed)
            {
                PlayerUpgradeEvents.OnUpgradePickaxeSpeed?.Invoke(false);
                _currentLevel = DataManager.PickaxeSpeedLevel;
            }

            CheckForForcedUpgrade();
        }
        private void RewardedAdUpgrade()
        {
            if (_upgradeItemType == UpgradeItemType.MoneyValue)
            {
                PlayerUpgradeEvents.OnUpgradeMoneyValue?.Invoke(true);
                _currentLevel = DataManager.MoneyValueLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxeDurability)
            {
                PlayerUpgradeEvents.OnUpgradePickaxeDurability?.Invoke(true);
                _currentLevel = DataManager.PickaxeDurabilityLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxePower)
            {
                PlayerUpgradeEvents.OnUpgradePickaxePower?.Invoke(true);
                _currentLevel = DataManager.PickaxePowerLevel;
            }
            else if (_upgradeItemType == UpgradeItemType.PickaxeSpeed)
            {
                PlayerUpgradeEvents.OnUpgradePickaxeSpeed?.Invoke(true);
                _currentLevel = DataManager.PickaxeSpeedLevel;
            }

            CheckForForcedUpgrade();

            Debug.Log("Rewarded Upgrade is Successful!");
        }
        #endregion

        #region PUBLICS
        public void CheckForMoneySufficiency()
        {
            int requiredMoney;
            if (_upgradeItemType == UpgradeItemType.MoneyValue)
                requiredMoney = DataManager.MoneyValueCost;
            else if (_upgradeItemType == UpgradeItemType.PickaxeDurability)
                requiredMoney = DataManager.PickaxeDurabilityCost;
            else if (_upgradeItemType == UpgradeItemType.PickaxePower)
                requiredMoney = DataManager.PickaxePowerCost;
            else if (_upgradeItemType == UpgradeItemType.PickaxeSpeed)
                requiredMoney = DataManager.PickaxeSpeedCost;
            else
            {
                requiredMoney = 0;
                Debug.Log("Unknown upgrade type!", this);
            }
            
            _upgradeButton.interactable = DataManager.TotalMoney >= requiredMoney;

            #region SOMETIMES INTERACTABLE COLOR DOES NOT WORK WITHOUT THIS
            _upgradeButton.transition = Selectable.Transition.None;
            _upgradeButton.transition = Selectable.Transition.ColorTint;
            #endregion
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartShakeLevelImageSequence()
        {
            DeleteShakeLevelImageSequence();
            CreateShakeLevelImageSequence();
            _shakeLevelImageSequence.Play();
        }
        private void CreateShakeLevelImageSequence()
        {
            if (_shakeLevelImageSequence == null)
            {
                _shakeLevelImageSequence = DOTween.Sequence();
                _shakeLevelImageSequenceID = Guid.NewGuid();
                _shakeLevelImageSequence.id = _shakeLevelImageSequenceID;

                _shakeLevelImageSequence.Append(_levelImage.DOShakeScale(SHAKE_DURATION, .5f))
                    .OnComplete(DeleteShakeLevelImageSequence);
            }
        }
        private void DeleteShakeLevelImageSequence()
        {
            DOTween.Kill(_shakeLevelImageSequenceID);
            _shakeLevelImageSequence = null;
        }
        #endregion
    }
}
