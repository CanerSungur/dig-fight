using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ExplosiveBox : MonoBehaviour, IHealth, IDamageable
    {
        [Header("-- SETUP --")]
        [SerializeField] private int hp = 1;
        [SerializeField] private LayerMask affectedLayer;

        private Player _player;
        private Transform _meshTransform;
        

        #region PROPERTIES
        public int MaxHealth => hp;
        public int CurrentHealth { get; set; }
        #endregion

        public void Init(Layer layer)
        {
            _meshTransform = transform.GetChild(0);
            CurrentHealth = MaxHealth;
        }

        private void AffectNearBoxes()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (Collider boxCollider in hitColliders)
            {
                if (boxCollider.TryGetComponent(out BreakableBox breakableBox))
                {
                    breakableBox.Explode();
                }
            }
        }

        #region INTERFACE FUNCTIONS
        public void GetDamaged(int amount)
        {
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);

            //if (amount <= CurrentHealth)
            //    CollectableEvents.OnSpawnMoney?.Invoke(amount, transform.position);
            //else
            //    CollectableEvents.OnSpawnMoney?.Invoke(CurrentHealth, transform.position);

            CurrentHealth -= amount;
            AudioManager.PlayAudio(Enums.AudioType.HitBox, 0.5f);

            if (CurrentHealth <= 0)
                Break();

            AffectNearBoxes();
        }
        public void Break()
        {
            if (_player == null)
                Debug.Log("Player is not assigned!");
            else
            {
                _player.StoppedDigging();
                _player.DigHandler.StopDiggingProcess();
            }

            gameObject.SetActive(false);
            AudioManager.PlayAudio(Enums.AudioType.HitExplosive, 0.3f);
            CameraManager.OnExplosiveHitShake?.Invoke();
        }
        #endregion

        #region PUBLICS
        public void AssignHitter(Player player)
        {
            _player = player;
        }
        #endregion
    }
}
