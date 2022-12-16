using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ExplosiveBox : MonoBehaviour, IHealth, IDamageable, IBoxInteractable
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

        #region INTERFACE FUNCTIONS
        public void Init(Layer layer)
        {
            _meshTransform = transform.GetChild(0);
            CurrentHealth = MaxHealth;
        }
        public void ChangeParent(Transform transform)
        {
            this.transform.SetParent(transform);
        }
        public void GetDamaged(int amount)
        {
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);

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

            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.ExplosionLargeEffect, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            AudioManager.PlayAudio(Enums.AudioType.HitExplosive, 0.3f);
            CameraManager.OnExplosiveHitShake?.Invoke();
            HapticEvents.OnPlayHitExplosive?.Invoke();

            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
        #endregion

        private void AffectNearBoxes()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (Collider boxCollider in hitColliders)
            {
                if (boxCollider.TryGetComponent(out BreakableBox breakableBox))
                    breakableBox.Explode();
                else if (boxCollider.TryGetComponent(out ChestBase chest))
                    chest.Explode();
            }
        }

        #region PUBLICS
        public void AssignHitter(Player player)
        {
            _player = player;
        }
        #endregion
    }
}
