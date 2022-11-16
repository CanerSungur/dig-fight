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

        // cost data
        private static readonly int _upgradeCost = 30;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;

        // core data
        private readonly float _coreMovementSpeed = 3f;
        private readonly float _coreMoneyValue = 1;

        // increment data
        private readonly float _movementSpeedIncrement = 0.2f;
        private readonly float _moneyValueIncrement = 0.5f;
        #endregion

        public static float TotalMoney { get; private set; }

        public void Init(GameManager gameManager)
        {
            LoadData();

            UpdateMovementSpeed();
            UpdateMoneyValue();

            PlayerUpgradeEvents.OnUpgradeMovementSpeed += MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue += MoneyValueUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnSpend += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnUpgradeMovementSpeed -= MovementSpeedUpgrade;
            PlayerUpgradeEvents.OnUpgradeMoneyValue -= MoneyValueUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnSpend -= DecreaseTotalMoney;

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
        #endregion

        #region UPGRADEABLE VALUE FUNCTIONS
        #region UPGRADE FUNCTIONS
        private void MovementSpeedUpgrade()
        {
            IncreaseMovementSpeedLevel();
            UpdateMovementSpeed();
            PlayerEvents.OnCheer?.Invoke();
        }
        private void MoneyValueUpgrade()
        {
            IncreaseMoneyValueLevel();
            UpdateMoneyValue();
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
        #endregion

        #region INCREMENT FUNCTIONS
        private void IncreaseMovementSpeedLevel()
        {
            if (TotalMoney >= MovementSpeedCost)
            {
                DecreaseTotalMoney(MovementSpeedCost);
                MovementSpeedLevel++;
                PlayerUpgradeEvents.OnUpdateUpgradeTexts?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseMoneyValueLevel()
        {
            if (TotalMoney >= MoneyValueCost)
            {
                DecreaseTotalMoney(MoneyValueCost);
                MoneyValueLevel++;
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
        }
        private void SaveData()
        {
            PlayerPrefs.SetFloat("TotalMoney", TotalMoney);
            PlayerPrefs.SetInt("MovementSpeedLevel", MovementSpeedLevel);
            PlayerPrefs.SetInt("MoneyValueLevel", MoneyValueLevel);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
