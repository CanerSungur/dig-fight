using System;
using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeDurabilityHandler : MonoBehaviour
    {
        [Header("-- BAR SETUP --")]
        [SerializeField] private PickaxeDurabilityBar _durabilityBar;

        private Pickaxe _pickaxe;

        private int _maxDurability;
        private int _currentDurability;

        #region PROPERTIES
        public int MaxDurability => _maxDurability;
        public int CurrentDurability => _currentDurability;
        public Pickaxe Pickaxe => _pickaxe;
        #endregion

        public void Init(Pickaxe pickaxe)
        {
            if (_pickaxe == null)
                _pickaxe = pickaxe;

            _maxDurability = DataManager.PickaxeDurability;
            _currentDurability = _maxDurability;
            _durabilityBar.Init(this);

            PlayerEvents.OnSetCurrentPickaxeDurability += UpdateDurability;
            PlayerEvents.OnTakePickaxeDurability += HandleDurabilityPickup;
        }

        private void OnDisable()
        {
            if (_pickaxe == null) return;
            PlayerEvents.OnSetCurrentPickaxeDurability -= UpdateDurability;
            PlayerEvents.OnTakePickaxeDurability -= HandleDurabilityPickup;
        }

        #region PUBLICS
        public void GetDamaged()
        {
            _currentDurability--;
            if (_currentDurability <= 0)
                Break();

            _durabilityBar.GetDamaged();
        }
        #endregion

        private void Break()
        {
            if (!_pickaxe.IsBroken)
                PickaxeEvents.OnBreak?.Invoke();
        }
        private void UpdateDurability()
        {
            _maxDurability = DataManager.PickaxeDurability;
            _currentDurability = _maxDurability;
            _durabilityBar.ResetBar();
        }
        private void HandleDurabilityPickup(int amount)
        {
            _currentDurability += amount;
            if (_currentDurability > _maxDurability)
                _currentDurability = _maxDurability;

            _durabilityBar.GetRepaired();
        }
    }
}
