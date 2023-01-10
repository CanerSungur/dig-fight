using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigFight
{
    public class PickaxeUpgradeManager : MonoBehaviour
    {
        #region FOR UPGRADEABLE VARIABLES
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
        private readonly int _corePickaxeDurability = 10;
        private readonly int _corePickaxePower = 1;

        // increment data
        private readonly int _pickaxeDurabilityIncrement = 1;
        private readonly int _pickaxePowerIncrement = 1;
        #endregion
    }
}
