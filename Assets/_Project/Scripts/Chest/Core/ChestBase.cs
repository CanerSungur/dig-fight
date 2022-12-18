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
        #endregion

        #region BOOST VALUE DATA
        //private const int CORE_DURABILITY_VALUE = 3;
        //private const float CORE_SPEED_VALUE = 2f;
        //private const int CORE_POWER_VALUE = 2;

        //private int _durabilityValue => CORE_DURABILITY_VALUE + LevelHandler.Level;
        //private float _speedValue => CORE_SPEED_VALUE + (LevelHandler.Level * 0.1f);
        //private int _powerValue => CORE_POWER_VALUE + (LevelHandler.Level);
        #endregion

        #region PROPERTIES
        public bool Triggered => _triggered;
        public PowerUp PowerUp => powerUp;
        public Enums.ChestType ChestType => _chestType;
        //public int DurabilityValue => _durabilityValue;
        //public float SpeedValue => _speedValue;
        //public int PowerValue => _powerValue;
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
        public void ChangeParent(Transform transform)
        {
            this.transform.SetParent(transform);
        }
        public void AssignInteracter(Player player)
        {
            _player = player;
        }
        #endregion

        #region PUBLICS
        //public void AssignHitter(Player player) => _player = player;
        public void GiveBoost()
        {
            StopHittersDiggingProcess();

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
                CameraManager.OnBoxBreakShake?.Invoke();
                HapticEvents.OnPlayBreakBox?.Invoke();

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
            if (_player == null)
                Debug.Log("Player is not assigned!");
            else
            {
                _player.StoppedDigging();
                _player.DigHandler.StopDiggingProcess();
            }
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartOpenChestSequence()
        {
            //Init();

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

                        //if (_chestType == Enums.ChestType.PickaxeDurability)
                        //    PowerUpEvents.OnPickaxeDurabilityTaken?.Invoke(this, _durabilityValue);
                        //else if (_chestType == Enums.ChestType.PickaxeSpeed)
                        //    PowerUpEvents.OnPickaxeSpeedTaken?.Invoke(this, _speedValue);
                        //else if (_chestType == Enums.ChestType.PickaxePower)
                        //    PowerUpEvents.OnPickaxePowerTaken?.Invoke(this, _powerValue);

                        //_animation.Play();
                        //CameraManager.OnBoostPickedUp?.Invoke();

                        DeleteOpenChestSequence();
                        //Destroy(gameObject);
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
        public void Dispose()
        {
            Destroy(gameObject, 0.5f);
        }
        #endregion

        public abstract void TriggerPickUp();
    }
}
