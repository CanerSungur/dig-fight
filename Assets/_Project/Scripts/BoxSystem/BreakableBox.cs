using DG.Tweening;
using UnityEngine;
using ZestGames;
using System;
using ZestCore.Utility;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class BreakableBox : MonoBehaviour, IHealth, IDamageable, IBoxInteractable
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxType _boxType;
        [SerializeField] private int hp = 1;
        private bool _isBroken;

        #region SCRIPT REFERENCES
        private Player _player;
        private Ai _ai;
        private DebrisHandler _debrisHandler;
        private CrackHandler _crackHandler;
        private PieceHandler _pieceHandler;
        #endregion

        #region COMPONENTS
        private Transform _meshTransform;
        private Collider _collider;
        #endregion

        #region EFFECT RELATED
        private readonly Vector3 vfxVoxelOffset = new Vector3(0f, 0f, -1.75f);
        private Enums.HitPower _affectedHitPower;
        #endregion

        #region PROPERTIES
        public int MaxHealth => hp;
        public int CurrentHealth { get; set; }
        public DebrisHandler DebrisHandler => _debrisHandler;
        public Enums.BoxType BoxType => _boxType;
        public Player Player => _player;
        public Ai Ai => _ai;
        #endregion

        #region SEQUENCE
        private Sequence _shakeSequence;
        private Guid _shakeSequenceID;
        private const float SHAKE_DURATION = 0.5f;
        #endregion

        #region INTERFACE FUNCTIONS
        public void Init(Layer layer)
        {
            CurrentHealth = MaxHealth;
            _isBroken = false;

            if (_meshTransform == null)
            {
                _meshTransform = transform.GetChild(0);
                _collider = GetComponent<Collider>();

                _debrisHandler = GetComponent<DebrisHandler>();
                _crackHandler = GetComponent<CrackHandler>();

                if (_boxType != Enums.BoxType.Stone)
                {
                    _pieceHandler = GetComponent<PieceHandler>();
                    _pieceHandler.Init(this);
                }
            }

            _collider.enabled = true;
            _crackHandler.Init(this);
            _debrisHandler.Init(this);
        }
        public void ChangeParent(Transform transform) => this.transform.SetParent(transform);
        public void AssignInteracter(Player player) => _player = player;
        public void AssignInteracter(Ai ai) => _ai = ai;
        public void GetDamaged(int amount)
        {
            if (_player)
                CameraManager.OnBoxHitShake?.Invoke();
            AudioManager.PlayAudio(Enums.AudioType.HitBox, 0.5f);

            SpawnMoneyOnHit(amount);

            SetHitPower(amount);
            SpawnEffect(_affectedHitPower);

            CurrentHealth -= amount;

            _crackHandler.EnhanceCracks();
            if (_pieceHandler != null)
                _pieceHandler.Release();

            if (CurrentHealth <= 0)
                Break();

            HapticEvents.OnPlayHitBox?.Invoke();
            StartShakeSequence();
        }
        public void Break()
        {
            _isBroken = true;
            AudioManager.PlayAudio(Enums.AudioType.BreakBox, 0.3f);
            if (_player)
            {
                CameraManager.OnBoxBreakShake?.Invoke();
                HapticEvents.OnPlayBreakBox?.Invoke();
            }

            _crackHandler.DisposeCracks();
            _debrisHandler.ActivateDebrises();
            _collider.enabled = false;

            StopHittersDiggingProcess();

            Destroy(gameObject);
        }
        #endregion

        #region PUBLICS
        public void Explode()
        {
            if (_isBroken) return;

            Delayer.DoActionAfterDelay(this, Random.Range(0f, 0.75f), () => {
                SpawnEffect(Enums.HitPower.High);

                if (_player)
                    CollectableEvents.OnSpawnMoney?.Invoke(CurrentHealth, transform.position);

                CurrentHealth = 0;
                AudioManager.PlayAudio(Enums.AudioType.BreakBox, 0.3f);
                if (_player)
                {
                    CameraManager.OnBoxBreakShake?.Invoke();
                    HapticEvents.OnPlayBreakBox?.Invoke();
                }

                if (_pieceHandler != null)
                    _pieceHandler.Release();

                _crackHandler.DisposeCracks();
                _debrisHandler.ActivateDebrises();

                Destroy(gameObject);
            });
        }
        public float GetCurrentHealthNormalized() => (((float)MaxHealth - CurrentHealth) / (float)MaxHealth);
        #endregion

        #region PRIVATES
        private void StopHittersDiggingProcess()
        {
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
        }
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
            vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Stone, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
            vfxVoxel.Init(affectedHitPower);
            //if (_boxType == Enums.BoxType.Stone)
            //{
            //    vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Stone, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
            //    vfxVoxel.Init(affectedHitPower);
            //}
            //else if (_boxType == Enums.BoxType.Copper)
            //{
            //    vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Copper, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
            //    vfxVoxel.Init(affectedHitPower);
            //}
            //else if (_boxType == Enums.BoxType.Diamond)
            //{
            //    vfxVoxel = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Voxel_Diamond, transform.position + vfxVoxelOffset, Quaternion.Euler(-90f, 0f, 0f)).GetComponent<VFXVoxel>();
            //    vfxVoxel.Init(affectedHitPower);
            //}
            //else
            //    Debug.Log("Unknown BOX TYPE!");

            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);
        }
        private void SpawnMoneyOnHit(int amount)
        {
            if (_ai && !_player) return;

            if (amount <= CurrentHealth)
                CollectableEvents.OnSpawnMoney?.Invoke(amount, transform.position);
            else
                CollectableEvents.OnSpawnMoney?.Invoke(CurrentHealth, transform.position);
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
