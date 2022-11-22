using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeDurabilityHandler : MonoBehaviour
    {
        private Pickaxe _pickaxe;

        private int _maxDurability = 5;
        private int _currentDurability;

        public void Init(Pickaxe pickaxe)
        {
            if (_pickaxe == null)
                _pickaxe = pickaxe;

            _currentDurability = _maxDurability;
        }

        #region PUBLICS
        public void GetDamaged()
        {
            _currentDurability--;
            if (_currentDurability <= 0)
                Break();
        }
        #endregion

        private void Break()
        {
            if (!_pickaxe.IsBroken)
                PickaxeEvents.OnBreak?.Invoke();
        }
    }
}
