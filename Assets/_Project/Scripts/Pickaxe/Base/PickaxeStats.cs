using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeStats : MonoBehaviour
    {
        private Pickaxe _pickaxe;
        private PickaxeUpgrade _durabilityUpgrade;
        private PickaxeUpgrade _powerUpgrade;

        #region PROPERTIES
        public int Durability { get; private set ;}
        public int Power { get; private set; }
        #endregion

        public void Init(Pickaxe pickaxe)
        {
            if (_pickaxe == null)
            {
                _pickaxe = pickaxe;
                _durabilityUpgrade = _pickaxe.PickaxeBase.GetUpgrade(Enums.PickaxeUpgradeType.Durability);
                _powerUpgrade = _pickaxe.PickaxeBase.GetUpgrade(Enums.PickaxeUpgradeType.Power);

                if (_durabilityUpgrade == null)
                    Debug.Log(gameObject.name + " Durability upgrade is null");
                if (_powerUpgrade == null)
                    Debug.Log(gameObject.name + " Power upgrade is null");

                _durabilityUpgrade.LoadLevel();
                _powerUpgrade.LoadLevel();

                UpdateDurability();
                UpdatePower();

                _durabilityUpgrade.OnUpgradeSuccessful += UpdateDurability;
                _powerUpgrade.OnUpgradeSuccessful += UpdatePower;
            }
        }

        private void OnDisable() 
        {
            if (_pickaxe == null) return;

            _durabilityUpgrade.OnUpgradeSuccessful -= UpdateDurability;
            _powerUpgrade.OnUpgradeSuccessful -= UpdatePower;
        }

        private void UpdateDurability()
        {
            Durability = _durabilityUpgrade.Value;
            // Debug.Log(_pickaxe.name + " durability level: " + _durabilityUpgrade.Level);
            // Debug.Log(_pickaxe.name + " durability price: " + _durabilityUpgrade.Price);
            // Debug.Log(_pickaxe.name + " durability: " + Durability);
        }
        private void UpdatePower()
        {
            Power = _powerUpgrade.Value;
            // Debug.Log(_pickaxe.name + " power level: " + _powerUpgrade.Level);
            // Debug.Log(_pickaxe.name + " power price: " + _powerUpgrade.Price);
            // Debug.Log(_pickaxe.name + " power: " + Power);
        }
    }
}
