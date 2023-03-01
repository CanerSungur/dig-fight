using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;
using ZestCore.Utility;
using Random = UnityEngine.Random;

namespace DigFight
{
    public abstract class ChestBase : MonoBehaviour, IBoxInteractable
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.ChestType _chestType;
        [SerializeField] protected float duration;
        [SerializeField] protected float incrementValue;

        private bool _triggered, _isBroken = false;
        protected PowerUp powerUp;

        #region COMPONENTS
        private Transform _coverTransform, _chestTransform;
        private RectTransform _shineRectTransform;
        private Animation _animation;
        private Collider _collider;
        private Player _player;
        private Ai _ai;
        #endregion

        #region PROPERTIES
        public bool Triggered => _triggered;
        public PowerUp PowerUp => powerUp;
        public Enums.ChestType ChestType => _chestType;
        #endregion

        #region SEQUENCE
        private Sequence _openChestSequence;
        private Guid _openChestSequenceID;

        private const float OPEN_CHEST_DURATION = 1.5f;
        #endregion

        #region INTERFACE FUNCTIONS
        public virtual void Init(Layer layer)
        {
            if (_coverTransform == null)
            {
                _coverTransform = transform.GetChild(0).GetChild(0);
                _chestTransform = transform.GetChild(0);
                _shineRectTransform = transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
                _shineRectTransform.localScale = Vector3.zero;
                _animation = GetComponent<Animation>();
                _animation.Rewind();
                _collider = GetComponent<Collider>();
            }
        }
        public void ChangeParent(Transform transform) => this.transform.SetParent(transform);
        public void AssignInteracter(Player player) => _player = player;
        public void AssignInteracter(Ai ai) => _ai = ai;
        #endregion

        #region PUBLICS
        public void GiveBoost()
        {
            StopHittersDiggingProcess();

            CoinEvents.OnSpawnCoin?.Invoke(1, transform.position);

            AudioManager.PlayAudio(Enums.AudioType.HitChest);
            AudioManager.PlayAudio(Enums.AudioType.ChestOpen);
            StartOpenChestSequence();

            _triggered = true;
        }
        public void Explode()
        {
            if (_isBroken) return;
            _isBroken = true;

            Delayer.DoActionAfterDelay(this, Random.Range(0f, 0.75f), () => {
                SpawnEffects();
        
                AudioManager.PlayAudio(Enums.AudioType.BreakChest, 0.3f);
                if (_player)
                {
                    CameraManager.OnBoxBreakShake?.Invoke();
                    HapticEvents.OnPlayBreakBox?.Invoke();
                }

                Destroy(gameObject);
            });
        }
        public void StartClosingChestSequence()
        {
            _animation.Play();
            Delayer.DoActionAfterDelay(this, _animation.clip.averageDuration - 0.25f, () => _collider.enabled = false);
        }
        #endregion

        #region HELPERS
        private void SpawnEffects()
        {
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.VFX_Chest_Break, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);
        }
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
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartOpenChestSequence()
        {
            CreateOpenChestSequence();
            _openChestSequence.Play();
        }
        private void CreateOpenChestSequence()
        {
            if (_openChestSequence == null)
            {
                _openChestSequence = DOTween.Sequence();
                _openChestSequenceID = Guid.NewGuid();
                _openChestSequence.id = _openChestSequenceID;

                _openChestSequence.Append(_chestTransform.DOShakeScale(OPEN_CHEST_DURATION * 0.5f, 0.25f))
                    .Append(_coverTransform.DOLocalRotate(new Vector3(-45f, 0f, 0f), OPEN_CHEST_DURATION)).SetEase(Ease.OutBounce)
                    .Join(_shineRectTransform.DOScale(Vector3.one, OPEN_CHEST_DURATION)).SetEase(Ease.OutBounce)
                    .Join(_chestTransform.DOScale(Vector3.one * 0.7f, OPEN_CHEST_DURATION))
                    .OnComplete(() => {
                        TriggerPickUp();
                        DeleteOpenChestSequence();
                    });
            }
        }
        private void DeleteOpenChestSequence()
        {
            DOTween.Kill(_openChestSequenceID);
            _openChestSequence = null;
        }
        #endregion

        #region ANIMATION EVENT FUNCTIONS
        public void Dispose() => Destroy(gameObject, 0.5f);
        #endregion

        public abstract void TriggerPickUp();
    }
}
