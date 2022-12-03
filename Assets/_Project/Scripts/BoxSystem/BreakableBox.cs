using DG.Tweening;
using UnityEngine;
using ZestGames;
using System;
using ZestCore.Utility;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class BreakableBox : MonoBehaviour, IHealth, IDamageable
    {
        //[Header("-- SETUP --")]
        //[SerializeField] private ParticleSystem hitSmokePS;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxType _boxType;
        [SerializeField] private int hp = 1;

        #region SCRIPT REFERENCES
        private Player _player;
        private DebrisHandler _debrisHandler;
        private CrackHandler _crackHandler;
        #endregion

        #region COMPONENTS
        private Transform _meshTransform;
        private Collider _collider;
        #endregion

        #region EFFECT RELATED
        private readonly Vector3 vfxVoxelOffset = new Vector3(0f, 1.5f, -1.75f);
        private Enums.HitPower _affectedHitPower;
        #endregion

        //private const int DEFAULT_HP = 1;

        #region PROPERTIES
        public int MaxHealth => hp;
        public int CurrentHealth { get; set; }
        public DebrisHandler DebrisHandler => _debrisHandler;
        #endregion

        #region SEQUENCE
        private Sequence _shakeSequence;
        private Guid _shakeSequenceID;
        private const float SHAKE_DURATION = 0.5f;
        #endregion

        public void Init(Layer layer)
        {
            CurrentHealth = MaxHealth;

            if (_meshTransform == null)
            {
                _meshTransform = transform.GetChild(0);
                _collider = GetComponent<Collider>();

                _debrisHandler = GetComponent<DebrisHandler>();
                _crackHandler = GetComponent<CrackHandler>();
            }

            _debrisHandler.Init(this);
            _crackHandler.Init(this);
        }

        #region INTERFACE FUNCTIONS
        public void GetDamaged(int amount)
        {
            CameraManager.OnBoxHitShake?.Invoke();

            if (amount <= CurrentHealth)
                CollectableEvents.OnSpawnMoney?.Invoke(amount, transform.position);
            else
                CollectableEvents.OnSpawnMoney?.Invoke(CurrentHealth, transform.position);

            SetHitPower(amount);
            SpawnEffect(_affectedHitPower);

            CurrentHealth -= amount;
            AudioManager.PlayAudio(Enums.AudioType.HitBox, 0.5f);
            _crackHandler.EnhanceCracks();

            if (CurrentHealth <= 0)
                Break();

            StartShakeSequence();
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

            _crackHandler.DisposeCracks();
            _collider.enabled = false;
            _debrisHandler.ReleaseDebris(_debrisHandler.TotalDebrisCount);
            AudioManager.PlayAudio(Enums.AudioType.BreakBox, 0.3f);
            CameraManager.OnBoxBreakShake?.Invoke();
            gameObject.SetActive(false);
        }
        #endregion

        #region PUBLICS
        public void AssignHitter(Player player)
        {
            _player = player;
        }
        public void Explode()
        {
            Delayer.DoActionAfterDelay(this, Random.Range(0f, 0.75f), () => {
                _crackHandler.DisposeCracks();
                SpawnEffect(Enums.HitPower.High);

                CollectableEvents.OnSpawnMoney?.Invoke(CurrentHealth, transform.position);
                CurrentHealth = 0;

                _collider.enabled = false;
                _debrisHandler.ReleaseDebris(_debrisHandler.TotalDebrisCount);
                
                AudioManager.PlayAudio(Enums.AudioType.BreakBox, 0.3f);
                CameraManager.OnBoxBreakShake?.Invoke();
                gameObject.SetActive(false);
            });
        }
        public float GetCurrentHealthNormalized() => (((float)MaxHealth - CurrentHealth) / (float)MaxHealth);
        #endregion

        #region PRIVATES
        private void SetHitPower(int damageAmount)
        {
            float damageRate = GetIncomingDamageNormalized(damageAmount);

            if (damageRate <= 0.25f)
                _affectedHitPower = Enums.HitPower.Low;
            else if (damageRate > 0.25f && damageRate <= .5f)
                _affectedHitPower = Enums.HitPower.Medium;
            else if (damageRate > .5f)
                _affectedHitPower = Enums.HitPower.High;
        }
        private float GetIncomingDamageNormalized(int damageAmount)
        {
            if (damageAmount >= CurrentHealth) return 1f;
            else return 1f - (((float)CurrentHealth - damageAmount) / (float)CurrentHealth);
        }
        private void SpawnEffect(Enums.HitPower affectedHitPower)
        {
            VFXVoxel vfxVoxel;
            if (_boxType == Enums.BoxType.Stone)
            {
                vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Stone, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
                vfxVoxel.Init(affectedHitPower);
            }
            else if (_boxType == Enums.BoxType.Copper)
            {
                vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Copper, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
                vfxVoxel.Init(affectedHitPower);
            }
            else if (_boxType == Enums.BoxType.Diamond)
            {
                vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Diamond, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
                vfxVoxel.Init(affectedHitPower);
            }
            else
                Debug.Log("Unknown BOX TYPE!");

            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartShakeSequence()
        {
            CreateShakeSequence();
            _shakeSequence.Play();
        }
        private void CreateShakeSequence()
        {
            if (_shakeSequence == null)
            {
                _shakeSequence = DOTween.Sequence();
                _shakeSequenceID = Guid.NewGuid();
                _shakeSequence.id = _shakeSequenceID;

                _shakeSequence.Append(_meshTransform.DOShakeScale(SHAKE_DURATION, 0.25f))
                    .Join(_meshTransform.DOShakeRotation(SHAKE_DURATION, 5f))
                    .OnComplete(DeleteShakeSequence);
            }
        }
        private void DeleteShakeSequence()
        {
            DOTween.Kill(_shakeSequenceID);
            _shakeSequence = null;
        }
        #endregion
    }
}
