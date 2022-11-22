using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeTrigger : MonoBehaviour
    {
        private Pickaxe _pickaxe;

        private void OnEnable()
        {
            if (_pickaxe == null)
                _pickaxe = GetComponentInParent<Pickaxe>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Box box) && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
            {
                if (_pickaxe.DamageHandler.Damage < box.CurrentHealth)
                    PlayerEvents.OnStagger?.Invoke();

                box.GetDamaged(_pickaxe.DamageHandler.Damage);
                _pickaxe.HitHappened();
                _pickaxe.DurabilityHandler.GetDamaged();
                PickaxeEvents.OnCannotHit?.Invoke();
            }
        }
    }
}
