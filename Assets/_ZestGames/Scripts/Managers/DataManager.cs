using System.ComponentModel;
using UnityEngine;

namespace ZestGames
{
    public class DataManager : MonoBehaviour
    {
        #region FOR UPGRADEABLE VARIABLES
        public static float MovementSpeed { get; private set; }
        public static int MovementSpeedLevel { get; private set; }
        public static int MovementSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, MovementSpeedLevel));
        // #####################
        public static float MoneyValue { get; private set; }
        public static int MoneyValueLevel { get; private set; }
        public static int MoneyValueCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, MoneyValueLevel));
        // #####################
        public static float PickaxeSpeed { get; private set; }
        public static int PickaxeSpeedLevel { get; private set; }
        public static int PickaxeSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, PickaxeSpeedLevel));
        // #####################
        public static int PickaxeDurability { get; private set; }
        public static int PickaxeDurabilityLevel { get; private set; }
        public static int PickaxeDurabilityCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, PickaxeDurabilityLevel));
        // #####################
        public static int PickaxePower { get; private set; }
        public static int PickaxePowerLevel { get; private set; }
        public static int PickaxePowerCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, PickaxePowerLevel));

        // cost data
        private static readonly int _upgradeCost = 30;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;

        // core data
        private readonly float _coreMovementSpeed = 3f;
        private readonly float _coreMoneyValue = 1;
        private readonly float _corePickaxeSpeed = 1.3f;
        private readonly int _corePickaxeDurability = 10;
        private readonly int _corePickaxePower = 1;

        // increment data
        private readonly float _movementSpeedIncrement = 0.2f;
        private readonly float _moneyValueIncrement = 0.5f;
        private readonly float _pickaxeSpeedIncrement = 0.2f;
        private readonly int _pickaxeDurabilityIncrement = 1;
        private readonly int _pickaxePowerIncrement = 1;
        #endregion

        public static float TotalMoney { get; private set; }
        public static int TotalCoin { get; private set; }

        public void Init(GameManager gameManager)
        {
            LoadData();

            UpdateMovementSpeed();
            UpdateMoneyValue();
            UpdatePickaxeSpeed();
            UpdatePickaxeDurability();
            UpdatePickaxePower();

            PlayerUpgradeEvents.OnUpgradeMovementSpeed += MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue += MoneyValueUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxeSpeed += PickaxeSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxeDurability += PickaxeDurabilityUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxePower += PickaxePowerUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnSpend += DecreaseTotalMoney;
            CoinEvents.OnCollect += IncreaseTotalCoin;
            CoinEvents.OnSpend += DecreaseTotalCoin;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpgradeMovementSpeed -= MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue -= MoneyValueUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxeSpeed -= PickaxeSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxeDurability -= PickaxeDurabilityUpgrade;
            PlayerUpgradeEvents.OnUpgradePickaxePower -= PickaxePowerUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnSpend -= DecreaseTotalMoney;
            CoinEvents.OnCollect -= IncreaseTotalCoin;
            CoinEvents.OnSpend -= DecreaseTotalCoin;

            SaveData();
        }

        #region FOR TESTING
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                CollectableEvents.OnCollect?.Invoke(1000);
            }
        }
#endif
        #endregion

        #region COLLECTABLE FUNCTIONS
        private void IncreaseTotalMoney(float amount)
        {
            TotalMoney += amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
        }
        private void DecreaseTotalMoney(float amount)
        {
            TotalMoney -= amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
        }
        private void IncreaseTotalCoin(int amount)
        {
            TotalCoin += amount;
            UiEvents.OnUpdateCoinText?.Invoke(TotalCoin);
        }
        private void DecreaseTotalCoin(int amount)
        {
            TotalCoin -= amount;
            UiEvents.OnUpdateCoinText?.Invoke(TotalCoin);
        }
        #endregion

        #region UPGRADEABLE VALUE FUNCTIONS
        #region UPGRADE FUNCTIONS
        private void MovementSpeedUpgrade(bool isItAd)
        {
            IncreaseMovementSpeedLevel(isItAd);
            UpdateMovementSpeed();
            PlayerEvents.OnCheer?.Invoke();
        }
        private void MoneyValueUpgrade(bool isItAd)
        {
            IncreaseMoneyValueLevel(isItAd);
            UpdateMoneyValue();
            PlayerEvents.OnCheer?.Invoke();
        }
        private void PickaxeSpeedUpgrade(bool isItAd)
        {
            IncreasePickaxeSpeedLevel(isItAd);
            UpdatePickaxeSpeed();
            PlayerEvents.OnCheer?.Invoke();
        }
        private void PickaxeDurabilityUpgrade(bool isItAd)
        {
            IncreasePickaxeDurabilityLevel(isItAd);
            UpdatePickaxeDurability();
            PlayerEvents.OnCheer?.Invoke();
        }
        private void PickaxePowerUpgrade(bool isItAd)
        {
            IncreasePickaxePowerLevel(isItAd);
            UpdatePickaxePower();
            PlayerEvents.OnCheer?.Invoke();
        }
        #endregion

        #region UPDATE FUNCTIONS
        private void UpdateMovementSpeed()
        {
            MovementSpeed = _coreMovementSpeed + _movementSpeedIncrement * (MovementSpeedLevel - 1);
            PlayerEvents.OnSetCurrentMovementSpeed?.Invoke();
        }
        private void UpdateMoneyValue()
        {
            MoneyValue = _coreMoneyValue + _moneyValueIncrement * (MoneyValueLevel - 1);
            PlayerEvents.OnSetCurrentMoneyValue?.Invoke();
        }
        private void UpdatePickaxeSpeed()
        {
            PickaxeSpeed = _corePickaxeSpeed + _pickaxeSpeedIncrement * (PickaxeSpeedLevel - 1);
            PlayerEvents.OnSetCurrentPickaxeSpeed?.Invoke();
        }
        private void UpdatePickaxeDurability()
        {
            PickaxeDurability = _corePickaxeDurability + _pickaxeDurabilityIncrement * (PickaxeDurabilityLevel - 1);
            PlayerEvents.OnSetCurrentPickaxeDurability?.Invoke();
        }
        private void UpdatePickaxePower()
        {
            PickaxePower = _corePickaxePower + _pickaxePowerIncrement * (PickaxePowerLevel - 1);
            PlayerEvents.OnSetCurrentPickaxePower?.Invoke();
        }
        #endregion

        #region INCREMENT FUNCTIONS
        private void IncreaseMovementSpeedLevel(bool isItAd)
        {
            if (isItAd)
            {
                MovementSpeed++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= MovementSpeedCost)
            {
                DecreaseTotalMoney(MovementSpeedCost);
                MovementSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseMoneyValueLevel(bool isItAd)
        {
            if (isItAd)
            {
                MoneyValueLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= MoneyValueCost)
            {
                DecreaseTotalMoney(MoneyValueCost);
                MoneyValueLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreasePickaxeSpeedLevel(bool isItAd)
        {
            if (isItAd)
            {
                PickaxeSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= PickaxeSpeedCost)
            {
                DecreaseTotalMoney(PickaxeSpeedCost);
                PickaxeSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreasePickaxeDurabilityLevel(bool isItAd)
        {
            if (isItAd)
            {
                PickaxeDurabilityLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= PickaxeDurabilityCost)
            {
                DecreaseTotalMoney(PickaxeDurabilityCost);
                PickaxeDurabilityLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreasePickaxePowerLevel(bool isItAd)
        {
            if (isItAd)
            {
                PickaxePowerLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= PickaxePowerCost)
            {
                DecreaseTotalMoney(PickaxePowerCost);
                PickaxePowerLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        #endregion
        #endregion

        #region LOAD-SAVE
        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);
            MovementSpeedLevel = PlayerPrefs.GetInt("MovementSpeedLevel", 1);
            MoneyValueLevel = PlayerPrefs.GetInt("MoneyValueLevel", 1);
            PickaxeSpeedLevel = PlayerPrefs.GetInt("PickaxeSpeedLevel", 1);
            PickaxeDurabilityLevel = PlayerPrefs.GetInt("PickaxeDurabilityLevel", 1);
            PickaxePowerLevel = PlayerPrefs.GetInt("PickaxePowerLevel", 1);
        }
        private void SaveData()
        {
            PlayerPrefs.SetFloat("TotalMoney", TotalMoney);
            PlayerPrefs.SetInt("MovementSpeedLevel", MovementSpeedLevel);
            PlayerPrefs.SetInt("MoneyValueLevel", MoneyValueLevel);
            PlayerPrefs.SetInt("PickaxeSpeedLevel", PickaxeSpeedLevel);
            PlayerPrefs.SetInt("PickaxeDurabilityLevel", PickaxeDurabilityLevel);
            PlayerPrefs.SetInt("PickaxePowerLevel", PickaxePowerLevel);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
