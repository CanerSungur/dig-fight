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
        private Ai _ai;
        private Transform _meshTransform;
        private Collider _collider;

        #region PROPERTIES
        public int MaxHealth => hp;
        public int CurrentHealth { get; set; }
        #endregion

        #region INTERFACE FUNCTIONS
        public void Init(Layer layer)
        {
            _meshTransform = transform.GetChild(0);
            _collider = GetComponent<Collider>();
            _collider.enabled = true;
            CurrentHealth = MaxHealth;
        }
        public void ChangeParent(Transform transform) => this.transform.SetParent(transform);
        public void AssignInteracter(Player player) => _player = player;
        public void AssignInteracter(Ai ai) => _ai = ai;
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
            _collider.enabled = false;

            if (_player)
            {
                _player.StoppedDigging();
                _player.DigHandler.StopDiggingProcess();
            }
            else if (_ai)
            {
                _ai.StoppedDigging();
                _ai.DigHandler.StopDiggingProcess();
                if (_ai.IsGrounded)
                    _ai.StateManager.SwitchState(_ai.StateManager.IdleState);
                else
                    _ai.StateManager.SwitchState(_ai.StateManager.FallState);
            }
            else
                Debug.Log("NO INTERACTOR is assigned!", this);

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
        //public void AssignHitter(Player player)
        //{
        //    _player = player;
        //}
        #endregion
    }
}
