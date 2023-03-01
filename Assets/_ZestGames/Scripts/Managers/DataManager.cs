using System.Data.Common;
using UnityEngine;
using DigFight;

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
        public static float DigSpeed { get; private set; }
        public static int DigSpeedLevel { get; private set; }
        public static int DigSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, DigSpeedLevel));

        // cost data
        private static readonly int _upgradeCost = 30;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;

        // core data
        private readonly float _coreMovementSpeed = 3f;
        private readonly float _coreMoneyValue = 1;
        private readonly float _coreDigSpeed = 1.3f;

        // increment data
        private readonly float _movementSpeedIncrement = 0.2f;
        private readonly float _moneyValueIncrement = 0.5f;
        private readonly float _digSpeedIncrement = 0.2f;
        
        #endregion

        public static float TotalMoney { get; private set; }
        public static int TotalCoin { get; private set; }

        #region PICKAXES
        public static bool HasRegularPickaxe { get; private set; }
        public static bool HasSilverPickaxe { get; private set; }
        public static bool HasGoldPickaxe { get; private set; }
        #endregion

        public void Init(GameManager gameManager)
        {
            LoadData();

            UpdateMovementSpeed();
            UpdateMoneyValue();
            UpdateDigSpeed();

            PlayerUpgradeEvents.OnUpgradeMovementSpeed += MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue += MoneyValueUpgrade;
            PlayerUpgradeEvents.OnUpgradeDigSpeed += DigSpeedUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnSpend += DecreaseTotalMoney;
            CoinEvents.OnCollect += IncreaseTotalCoin;
            CoinEvents.OnSpend += DecreaseTotalCoin;

            PlayerEvents.OnPurchaseSilverPickaxe += PurchaseSilverPickaxe;
            PlayerEvents.OnPurchaseGoldPickaxe += PurchaseGoldPickaxe;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpgradeMovementSpeed -= MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue -= MoneyValueUpgrade;
            PlayerUpgradeEvents.OnUpgradeDigSpeed -= DigSpeedUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnSpend -= DecreaseTotalMoney;
            CoinEvents.OnCollect -= IncreaseTotalCoin;
            CoinEvents.OnSpend -= DecreaseTotalCoin;

            PlayerEvents.OnPurchaseSilverPickaxe -= PurchaseSilverPickaxe;
            PlayerEvents.OnPurchaseGoldPickaxe -= PurchaseGoldPickaxe;

            SaveData();
        }

        #region PICKAXE FUNCTIONS
        private void PurchaseSilverPickaxe() => HasSilverPickaxe = true;
        private void PurchaseGoldPickaxe() => HasGoldPickaxe = true;
        #endregion

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
            UiEvents.OnUpdateMoneyText?.Invoke(TotalMoney);
            PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();

            ShopCanvas.OnCollectMoney?.Invoke(amount);
        }
        private void DecreaseTotalMoney(float amount)
        {
            TotalMoney -= amount;
            UiEvents.OnUpdateMoneyText?.Invoke(TotalMoney);
            PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();

            PickaxeUpgradeCanvas.OnSpendMoney?.Invoke(amount);
            ShopCanvas.OnSpendMoney?.Invoke(amount);
        }
        private void IncreaseTotalCoin(int amount)
        {
            TotalCoin += amount;
            UiEvents.OnUpdateCoinText?.Invoke(TotalCoin);

            ShopCanvas.OnCollectCoin?.Invoke(amount);

            Debug.Log("Coin: " + TotalCoin);
        }
        private void DecreaseTotalCoin(int amount)
        {
            TotalCoin -= amount;
            UiEvents.OnUpdateCoinText?.Invoke(TotalCoin);

            PickaxeUpgradeCanvas.OnSpendCoin?.Invoke(amount);
            ShopCanvas.OnSpendCoin?.Invoke(amount);
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
        private void DigSpeedUpgrade(bool isItAd)
        {
            IncreaseDigSpeedLevel(isItAd);
            UpdateDigSpeed();
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
        private void UpdateDigSpeed()
        {
            DigSpeed = _coreDigSpeed + _digSpeedIncrement * (DigSpeedLevel - 1);
            PlayerEvents.OnSetCurrentDigSpeed?.Invoke();
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
                UiEvents.OnUpdateMoneyText?.Invoke(TotalMoney);
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
                UiEvents.OnUpdateMoneyText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseDigSpeedLevel(bool isItAd)
        {
            if (isItAd)
            {
                DigSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                return;
            }

            if (TotalMoney >= DigSpeedCost)
            {
                DecreaseTotalMoney(DigSpeedCost);
                DigSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateMoneyText?.Invoke(TotalMoney);
            }
        }
        #endregion
        #endregion

        #region LOAD-SAVE
        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);
            TotalCoin = PlayerPrefs.GetInt("TotalCoin", 0);
            MovementSpeedLevel = PlayerPrefs.GetInt("MovementSpeedLevel", 1);
            MoneyValueLevel = PlayerPrefs.GetInt("MoneyValueLevel", 1);
            DigSpeedLevel = PlayerPrefs.GetInt("DigSpeedLevel", 1);

            HasRegularPickaxe = PlayerPrefs.GetInt("HasRegularPickaxe", 1) == 1;
            HasSilverPickaxe = PlayerPrefs.GetInt("HasSilverPickaxe", 0) == 1;
            HasGoldPickaxe = PlayerPrefs.GetInt("HasGoldPickaxe", 0) == 1;
        }
        private void SaveData()
        {
            PlayerPrefs.SetFloat("TotalMoney", TotalMoney);
            PlayerPrefs.SetInt("TotalCoin", TotalCoin);
            PlayerPrefs.SetInt("MovementSpeedLevel", MovementSpeedLevel);
            PlayerPrefs.SetInt("MoneyValueLevel", MoneyValueLevel);
            PlayerPrefs.SetInt("DigSpeedLevel", DigSpeedLevel);

            PlayerPrefs.SetInt("HasRegularPickaxe", HasRegularPickaxe == true ? 1 : 0);
            PlayerPrefs.SetInt("HasSilverPickaxe", HasSilverPickaxe == true ? 1 : 0);
            PlayerPrefs.SetInt("HasGoldPickaxe", HasGoldPickaxe == true ? 1 : 0);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
