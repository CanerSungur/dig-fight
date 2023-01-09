using UnityEngine;

namespace DigFight
{
    [System.Serializable]
    public struct Item
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

        public bool IsPurchased;
        public bool CanAfford;
    }
}
