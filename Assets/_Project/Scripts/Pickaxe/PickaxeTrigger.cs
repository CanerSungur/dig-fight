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
            if (other.TryGetComponent(out BreakableBox box) && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
            {
                if (_pickaxe.DamageHandler.Damage < box.CurrentHealth)
                    PlayerEvents.OnStagger?.Invoke();

                box.GetDamaged(_pickaxe.DamageHandler.Damage);
                _pickaxe.HitHappened();
                _pickaxe.DurabilityHandler.GetDamaged();
                PickaxeEvents.OnCannotHit?.Invoke();
            }

            if (other.TryGetComponent(out ExplosiveBox explosiveBox) && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
            {
                if (_pickaxe.DamageHandler.Damage < explosiveBox.CurrentHealth)
                    PlayerEvents.OnStagger?.Invoke();

                explosiveBox.GetDamaged(_pickaxe.DamageHandler.Damage);
                _pickaxe.HitHappened();
                _pickaxe.DurabilityHandler.GetDamaged();
                PickaxeEvents.OnCannotHit?.Invoke();
            }
        }
    }
}
