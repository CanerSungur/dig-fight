using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class Chest : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.ChestType chestType;
        private bool _triggered = false;
        private Transform _coverTransform, _chestTransform;
        private RectTransform _shineRectTransform;
        private Animation _animation;

        private const int CORE_DURABILITY_VALUE = 3;
        private const float CORE_SPEED_VALUE = 2f;

        private int _durabilityValue => CORE_DURABILITY_VALUE + LevelHandler.Level;
        private float _speedValue => CORE_SPEED_VALUE + (LevelHandler.Level * 0.1f);

        public bool Triggered => _triggered;

        #region SEQUENCE
        private Sequence _openChestSequence;
        private Guid _openChestSequenceID;

        private const float OPEN_CHEST_DURATION = 1.5f;
        #endregion

        private void Init()
        {
            if (_coverTransform == null)
            {
                _coverTransform = transform.GetChild(0).GetChild(0);
                _chestTransform = transform.GetChild(0);
                _shineRectTransform = transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
                _shineRectTransform.localScale = Vector3.zero;
                _animation = GetComponent<Animation>();
                _animation.Rewind();
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.TryGetComponent(out Player player) && !_triggered)
        //    {
        //        Debug.Log("hit");
        //        if (chestType == Enums.ChestType.PickaxeDurability)
        //        {
        //            PlayerEvents.OnTakePickaxeDurability?.Invoke(_durabilityValue);
        //            Destroy(gameObject);
        //        }
        //        else if (chestType == Enums.ChestType.PickaxeSpeed)
        //        {
        //            PlayerEvents.OnTakePickaxeSpeed?.Invoke(_speedValue);
        //            Debug.Log("pickup speed");
        //        }
        //    }
        //}

        #region PUBLICS
        public void GiveBoost()
        {
            AudioManager.PlayAudio(Enums.AudioType.HitChest);
            AudioManager.PlayAudio(Enums.AudioType.ChestOpen);
            StartOpenChestSequence();

            _triggered = true;
        }
        public void Explode()
        {

        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartOpenChestSequence()
        {
            Init();

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
                    .OnComplete(() => {
                        if (chestType == Enums.ChestType.PickaxeDurability)
                        {
                            PlayerEvents.OnTakePickaxeDurability?.Invoke(_durabilityValue);
                        }
                        else if (chestType == Enums.ChestType.PickaxeSpeed)
                        {
                            PlayerEvents.OnTakePickaxeSpeed?.Invoke(_speedValue);
                        }

                        _animation.Play();
                        CameraManager.OnBoostPickedUp?.Invoke();

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
        public void Dispos()
        {
            Destroy(gameObject, 0.5f);
        }
        #endregion
    }
}
