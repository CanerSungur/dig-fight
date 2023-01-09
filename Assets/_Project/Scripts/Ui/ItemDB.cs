using UnityEngine;

namespace DigFight
{
    [CreateAssetMenu(fileName = "Assets/Resources/_GameData/ShopItemDB", menuName = "Shopping/Shop Items Database")]
    public class ItemDB : ScriptableObject
    {
        [SerializeField] private Item[] _items;

        public int ShopItemCount => _items.Length;

        public Item GetShopItem(int index)
        {
            return _items[index];
        }
        public void PurchaseShopItem(int index)
        {
            _items[index].IsPurchased = true;
        }
    }
}
