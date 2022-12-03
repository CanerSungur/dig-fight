using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeDamageHandler : MonoBehaviour
    {
        private Pickaxe _pickaxe;
        private int _damage = 1;
        public int Damage => _damage;

        public void Init(Pickaxe pickaxe)
        {
            if (_pickaxe == null)
                _pickaxe = pickaxe;

            UpdateDamage();

            PlayerEvents.OnSetCurrentPickaxePower += UpdateDamage;
        }

        private void OnDisable()
        {
            if (_pickaxe == null) return;
            PlayerEvents.OnSetCurrentPickaxePower -= UpdateDamage;
        }

        private void UpdateDamage() => _damage = DataManager.PickaxePower;
    }
}
