using UnityEngine;
using ZestGames;

namespace DigFight
{
    [CreateAssetMenu(fileName = "Assets/Resources/_GameData/Pickaxe", menuName = "Game Data/new Pickaxe")]
    public class PickaxeBase : ScriptableObject
    {
        private bool _isPurchased;
        [SerializeField] private Enums.PickaxeType _pickaxeType;
        [SerializeField] private PickaxeUpgrade[] _upgrades;

        #region GETTERS
        public PickaxeUpgrade[] Upgrades => _upgrades;
        #endregion

        public PickaxeUpgrade GetUpgrade(Enums.PickaxeUpgradeType upgradeType)
        {
            PickaxeUpgrade upgrade = null;

            for (int i = 0; i < _upgrades.Length; i++)
            {
                if (_upgrades[i].UpgradeType == upgradeType)
                    upgrade = _upgrades[i];
            }

            return upgrade;
        }
    }
}
