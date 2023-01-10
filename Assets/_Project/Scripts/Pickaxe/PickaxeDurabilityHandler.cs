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

        private bool _isItPlayer = false;
        private int _maxDurability;
        private int _currentDurability;

        #region PROPERTIES
        public int MaxDurability => _maxDurability;
        public int CurrentDurability => _currentDurability;
        public Pickaxe Pickaxe => _pickaxe;
        #endregion

        public void Init(Pickaxe pickaxe, bool isItPlayer)
        {
            if (_pickaxe == null)
            {
                _pickaxe = pickaxe;
                _isItPlayer = isItPlayer;
            }
            _maxDurability = _isItPlayer == true ? _pickaxe.Stats.Durability : AiStats.PickaxeDurability;
            _currentDurability = _maxDurability;

            if (_isItPlayer)
            {
                PlayerEvents.OnSetCurrentPickaxeDurability += UpdateDurability;
                PlayerEvents.OnActivatePickaxeDurability += HandleDurabilityPickup;
                PlayerEvents.OnRevive += ResetPickaxe;
                _durabilityBar.Init(this);
            }
            else
            {
                AiEvents.OnSetCurrentPickaxeDurability += UpdateDurability;
                AiEvents.OnActivatePickaxeDurability += HandleDurabilityPickup;
            }
        }

        private void OnDisable()
        {
            if (_pickaxe == null) return;

            if (_isItPlayer)
            {
                PlayerEvents.OnSetCurrentPickaxeDurability -= UpdateDurability;
                PlayerEvents.OnActivatePickaxeDurability -= HandleDurabilityPickup;
                PlayerEvents.OnRevive -= ResetPickaxe;
            }
            else
            {
                AiEvents.OnSetCurrentPickaxeDurability -= UpdateDurability;
                AiEvents.OnActivatePickaxeDurability -= HandleDurabilityPickup;
            }
        }

        #region PUBLICS
        public void GetDamaged()
        {
            _currentDurability--;
            if (_currentDurability <= 0)
                Break();
            if (_isItPlayer) _durabilityBar.GetDamaged();
        }
        public void GetDamagedForExplosive()
        {
            _currentDurability -= (int)(MaxDurability * 0.5f);
            if (_currentDurability <= 0)
                Break();
            if (_isItPlayer) _durabilityBar.GetDamaged();
        }
        #endregion

        private void Break()
        {
            if (!_pickaxe.IsBroken)
                _pickaxe.OnBreak?.Invoke();
        }
        private void ResetPickaxe()
        {
            _maxDurability = _isItPlayer == true ? _pickaxe.Stats.Durability : AiStats.PickaxeDurability;
            _currentDurability = _maxDurability;
            _durabilityBar.ResetBar();
        }

        #region EVENT HANDLER FUNCTIONS
        private void UpdateDurability()
        {
            _maxDurability = _isItPlayer == true ? _pickaxe.Stats.Durability : AiStats.PickaxeDurability;
            _currentDurability = _maxDurability;
            if (_isItPlayer) _durabilityBar.UpdateBar();
        }
        private void HandleDurabilityPickup(PowerUp powerUp)
        {
            _currentDurability += (int)powerUp.IncrementValue;
            if (_currentDurability > _maxDurability)
                _currentDurability = _maxDurability;

            if (_isItPlayer) _durabilityBar.GetRepaired();
        }
        #endregion
    }
}
