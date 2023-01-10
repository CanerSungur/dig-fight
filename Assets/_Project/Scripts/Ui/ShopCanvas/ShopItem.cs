using UnityEngine;

namespace DigFight
{
    [System.Serializable]
    public class ShopItem
    {
        public enum ItemTypeEnum { NotAssigned, PurchaseCoin, PurchasePickaxe }
        public enum PriceTypeEnum { Money, Coin }

        public Sprite Image;
        public string Header;
        public string Name;
        public ItemTypeEnum ItemType;
        public PriceTypeEnum PriceType;
        public int Price;
        public int PurchaseAmount;

        public bool IsPurchased { get; private set;}
        public bool CanAfford;

        public void PurchaseSuccessful()
        {
            IsPurchased = true;
            SavePurchaseInfo();
        }
        public void LoadPurchaseInfo()
        {
            if (ItemType == ItemTypeEnum.PurchaseCoin) return;

            IsPurchased = PlayerPrefs.GetInt($"{Name}_Purchased", 0) == 1;
        }
        private void SavePurchaseInfo()
        {
            if (ItemType == ItemTypeEnum.PurchaseCoin) return;

            PlayerPrefs.SetInt($"{Name}_Purchased", IsPurchased == true ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
