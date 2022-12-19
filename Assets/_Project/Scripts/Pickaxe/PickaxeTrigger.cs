using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PickaxeTrigger : MonoBehaviour
    {
        private Pickaxe _pickaxe;
        private bool _isItPlayer = false;
        private void OnEnable()
        {
            if (_pickaxe == null)
            {
                _pickaxe = GetComponentInParent<Pickaxe>();
                _isItPlayer = GetComponentInParent<Player>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isItPlayer)
            {
                #region PLAYER
                if (other.TryGetComponent(out BreakableBox box) && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
                {
                    if (_pickaxe.DamageHandler.Damage < box.CurrentHealth)
                        PlayerEvents.OnStagger?.Invoke();

                    box.GetDamaged(_pickaxe.DamageHandler.Damage);
                    _pickaxe.HitHappened();
                    _pickaxe.DurabilityHandler.GetDamaged();
                    _pickaxe.OnCannotHit?.Invoke();
                }

                if (other.TryGetComponent(out ExplosiveBox explosiveBox) && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
                {
                    if (_pickaxe.DamageHandler.Damage < explosiveBox.CurrentHealth)
                        PlayerEvents.OnStagger?.Invoke();

                    explosiveBox.GetDamaged(_pickaxe.DamageHandler.Damage);
                    _pickaxe.HitHappened();
                    _pickaxe.DurabilityHandler.GetDamaged();
                    _pickaxe.OnCannotHit?.Invoke();
                }

                if (other.TryGetComponent(out ChestBase chest) && !chest.Triggered && _pickaxe.Player.IsDigging && _pickaxe.CanHit)
                    chest.GiveBoost();
                #endregion
            }
            else
            {
                #region AI
                if (other.TryGetComponent(out BreakableBox aiBox) && _pickaxe.Ai.IsDigging && _pickaxe.CanHit)
                {
                    if (_pickaxe.DamageHandler.Damage < aiBox.CurrentHealth)
                        AiEvents.OnStagger?.Invoke();

                    aiBox.GetDamaged(_pickaxe.DamageHandler.Damage);
                    _pickaxe.HitHappened();
                    _pickaxe.DurabilityHandler.GetDamaged();
                    _pickaxe.OnCannotHit?.Invoke();
                }

                if (other.TryGetComponent(out ExplosiveBox aiExplosiveBox) && _pickaxe.Ai.IsDigging && _pickaxe.CanHit)
                {
                    if (_pickaxe.DamageHandler.Damage < aiExplosiveBox.CurrentHealth)
                        AiEvents.OnStagger?.Invoke();

                    aiExplosiveBox.GetDamaged(_pickaxe.DamageHandler.Damage);
                    _pickaxe.HitHappened();
                    _pickaxe.DurabilityHandler.GetDamaged();
                    _pickaxe.OnCannotHit?.Invoke();
                }

                if (other.TryGetComponent(out ChestBase aiChest) && !aiChest.Triggered && _pickaxe.Ai.IsDigging && _pickaxe.CanHit)
                    aiChest.GiveBoost();
                #endregion
            }
        }
    }
}
