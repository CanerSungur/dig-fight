using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeDamageHandler : MonoBehaviour
    {
        #region COMPONENTS
        private Pickaxe _pickaxe;
        #endregion

        private bool _isItPlayer = false;
        private int _damage = 1;
        public int Damage => _damage;

        public void Init(Pickaxe pickaxe, bool isItPlayer)
        {
            if (_pickaxe == null)
            {
                _pickaxe = pickaxe;
                _isItPlayer = isItPlayer;
            }

            UpdateDamage();

            if (_isItPlayer)
                PlayerEvents.OnSetCurrentPickaxePower += UpdateDamage;
            else
                AiEvents.OnSetCurrentPickaxePower += UpdateDamage;
        }

        private void OnDisable()
        {
            if (_pickaxe == null) return;
            if (_isItPlayer)
                PlayerEvents.OnSetCurrentPickaxePower -= UpdateDamage;
            else
                AiEvents.OnSetCurrentPickaxePower -= UpdateDamage;
        }

        private void UpdateDamage() => _damage = _isItPlayer == true ? DataManager.PickaxePower + (DataManager.PickaxePower * _pickaxe.Player.PowerUpHandler.PowerRate)
            : AiStats.PickaxePower + (AiStats.PickaxePower * _pickaxe.Ai.PowerUpHandler.PowerRate);
    }
}
