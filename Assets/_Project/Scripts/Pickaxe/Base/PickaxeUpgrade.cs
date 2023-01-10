using UnityEngine;
using ZestGames;
using System;

namespace DigFight
{
    [System.Serializable]
    public class PickaxeUpgrade
    {
        #region EVENTS
        public event Action OnUpgradeSuccessful;
        #endregion

        public Enums.PickaxeType OwnerType;
        public Enums.PickaxeUpgradeType UpgradeType;
        public enum PriceTypeEnum { Money, Coin }

        #region BASE
        public Sprite Image;
        public string Name;
        public string Title;
        public string Description;
        #endregion

        #region PRICE
        public PriceTypeEnum PriceType;
        public int BaseUpgradeCost;
        [Range(1.2f, 3f)]public float UpgradeCostIncreaseRate;
        public int Price => (int)(BaseUpgradeCost * Mathf.Pow(UpgradeCostIncreaseRate, Level));
        public bool CanAfford;
        #endregion

        #region UPGRADE VALUES
        public int Value => CoreValue + ValueIncrement * (Level - 1);
        public int CoreValue;
        public int ValueIncrement;
        #endregion

        public int Level { get; private set; }

        #region PRIVATE FUNCTIONS
        private void SaveLevel()
        {
            PlayerPrefs.SetInt($"{Name}_{Title}_Level", Level);
            PlayerPrefs.Save();
        }
        #endregion

        #region PUBLIC FUNCTIONS
        public void Upgrade()
        {
            Level++;
            SaveLevel();

            OnUpgradeSuccessful?.Invoke();
            Debug.Log("Upgrade Happened!");
        }
        public void LoadLevel()
        {
            Level = PlayerPrefs.GetInt($"{Name}_{Title}_Level", 1);
        }
        #endregion    
    }
}
