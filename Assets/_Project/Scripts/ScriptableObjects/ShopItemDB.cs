using UnityEngine;

namespace DigFight
{
    [CreateAssetMenu(fileName = "Assets/Resources/_GameData/ShopItemDB", menuName = "Game Data/Shop Items Database")]
    public class ShopItemDB : ScriptableObject
    {
        [SerializeField] private ShopItem[] _items;

        public int ShopItemCount => _items.Length;

        public ShopItem GetShopItem(int index)
        {
            return _items[index];
        }
    }
}
