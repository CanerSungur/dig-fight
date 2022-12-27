using UnityEngine;
using ZestGames;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

namespace DigFight
{
    public class PickaxeDurabilityBar : MonoBehaviour
    {
        public enum SequenceType
        {
            Shake,
            Enable,
            Disable,
            GetDamaged,
            GetRepaired,
            FillRemainingDurability
        }

        private PickaxeDurabilityHandler _durabilityHandler;

        [Header("-- COLOR SETUP --")]
        [SerializeField] private Color _damagedColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _damageFlashColor;
        [SerializeField] private Color _healFlashColor;
        [SerializeField] private Color _healColor;
        [SerializeField] private ParticleSystem _glowPS;

        private Image _remainingDurabilityImage, _changedDurabilityImage;
        private Animation _pickaxeEnableAnim;

        #region SEQUENCE
        private Sequence _shakeSequence, _getDamagedSequence, _disableSequence, _enableSequence, _getRepairedSequence, _fillRemainingDurabilitySequence;
        private Guid _shakeSequenceID, _getDamagedSequenceID, _disableSequenceID, _enableSequenceID, _getRepairedSequenceID, _fillRemainingDurabilitySequenceID;

        private const float FADE_DURATION = 1.5f;
        private const float DAMAGED_COLOR_CHANGE_DURATION = 1f;
        private const float FLASH_COLOR_CHANGE_DURATION = 0.2f;

        private const float TOGGLE_DURATION = 0.5f;
        private const float DEACTIVATION_TIMER = 3f;
        private float _deactivationCountdown;
        #endregion

        public void Init(PickaxeDurabilityHandler durabilityHandler)
        {
            if (_durabilityHandler == null)
            {
                _durabilityHandler = durabilityHandler;
                _changedDurabilityImage = transform.GetChild(2).GetComponent<Image>();
                _remainingDurabilityImage = transform.GetChild(3).GetComponent<Image>();
                _pickaxeEnableAnim = transform.GetChild(0).GetChild(1).GetComponent<Animation>();
            }

            _changedDurabilityImage.fillAmount = _remainingDurabilityImage.fillAmount = GetDurabilityNormalized();
            _changedDurabilityImage.color = _damagedColor;
            _deactivationCountdown = DEACTIVATION_TIMER;

            transform.localScale = Vector3.zero;
            _glowPS.Stop();

            GameEvents.OnGameStart += EnableBar;
            GameEvents.OnGameEnd += DisableBar;
        }

        private void OnDisable()
        {
            if (_durabilityHandler == null) return;
            GameEvents.OnGameStart -= EnableBar;
            GameEvents.OnGameEnd -= DisableBar;
        }

        #region PRIVATES
        private void EnableBar() => StartEnableSequence();
        private void DisableBar(Enums.GameEnd gameEnd) => StartDisableSequence();
        private float GetDurabilityNormalized() => (float)_durabilityHandler.CurrentDurability / _durabilityHandler.MaxDurability;
        #endregion

        #region PUBLICS
        public void GetDamaged()
        {
            _changedDurabilityImage.fillAmount = _remainingDurabilityImage.fillAmount;

            //StartShakeSequence();
            StartGetDamagedSequence();

            _remainingDurabilityImage.fillAmount = GetDurabilityNormalized();
        }
        public void GetRepaired()
        {
            _changedDurabilityImage.fillAmount = GetDurabilityNormalized();

            //StartShakeSequence();
            StartGetRepairedSequence();
        }
        public void ResetBar() => _changedDurabilityImage.fillAmount = _remainingDurabilityImage.fillAmount = GetDurabilityNormalized();
        #endregion

        #region DOTWEEN FUNCTIONS

        #region GET REPAIRED
        private void StartGetRepairedSequence()
        {
            DeleteGetRepairedSequence();
            CreateGetRepairedSequence();
            _getRepairedSequence.Play();
        }
        private void CreateGetRepairedSequence()
        {
            if (_getRepairedSequence == null)
            {
                _getRepairedSequence = DOTween.Sequence();
                _getRepairedSequenceID = Guid.NewGuid();
                _getRepairedSequence.id = _getRepairedSequenceID;

                _getRepairedSequence.Append(DOVirtual.Color(_defaultColor, _healFlashColor, FLASH_COLOR_CHANGE_DURATION, r =>
                {
                    _changedDurabilityImage.color = r;
                }))
                    .Append(DOVirtual.Float(_remainingDurabilityImage.fillAmount, _changedDurabilityImage.fillAmount, FADE_DURATION, r =>
                    {
                        _remainingDurabilityImage.fillAmount = r;
                    }))
                    .OnComplete(() => {
                        DeleteGetRepairedSequence();
                    });
            }
        }
        private void DeleteGetRepairedSequence()
        {
            DOTween.Kill(_getRepairedSequenceID);
            _getRepairedSequence = null;
        }
        #endregion

        #region ENABLE
        private void StartEnableSequence()
        {
            DeleteEnableSequence();
            CreateEnableSequence();
            _enableSequence.Play();
        }
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequence;

                _enableSequence.Append(DOVirtual.Vector3(Vector3.zero, Vector3.one, TOGGLE_DURATION, r => {
                    transform.localScale = r;
                }))
                    .Append(transform.DOShakeScale(0.5f, 0.5f, 3, 50f))
                        .OnComplete(() =>
                        {
                            transform.localScale = Vector3.one;
                            _glowPS.Play();
                            _pickaxeEnableAnim.Play();
                            DeleteEnableSequence();
                        });
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        #endregion

        #region DISABLE
        private void StartDisableSequence()
        {
            DeleteDisableSequence();
            CreateDisableSequence();
            _disableSequence.Play();
        }
        private void CreateDisableSequence()
        {
            if (_disableSequence == null)
            {
                _disableSequence = DOTween.Sequence();
                _disableSequenceID = Guid.NewGuid();
                _disableSequence.id = _disableSequenceID;

                _glowPS.Stop();
                _disableSequence.Append(transform.DOShakeScale(0.5f, 0.5f, 3, 50f))
                    .Append(DOVirtual.Vector3(Vector3.one, Vector3.zero, TOGGLE_DURATION, r => {
                        transform.localScale = r;
                    }))

                        .OnComplete(() =>
                        {
                            transform.localScale = Vector3.zero;
                            DeleteDisableSequence();
                        });
            }
        }
        private void DeleteDisableSequence()
        {
            DOTween.Kill(_disableSequenceID);
            _disableSequence = null;
        }
        #endregion

        #region SHAKE
        private void StartShakeSequence()
        {
            DeleteShakeSequence();
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

                _shakeSequence.Append(transform.DOShakeRotation(1f, 20f, 5, 50))
                    .OnComplete(() => {
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                        DeleteShakeSequence();
                    });
            }
        }
        private void DeleteShakeSequence()
        {
            DOTween.Kill(_shakeSequenceID);
            _shakeSequence = null;
        }
        #endregion

        #region GET DAMAGED
        private void StartGetDamagedSequence()
        {
            DeleteGetDamagedSequence();
            CreateGetDamagedSequence();
            _getDamagedSequence.Play();
        }
        private void CreateGetDamagedSequence()
        {
            if (_getDamagedSequence == null)
            {
                _getDamagedSequence = DOTween.Sequence();
                _getDamagedSequenceID = Guid.NewGuid();
                _getDamagedSequence.id = _getDamagedSequenceID;

                _getDamagedSequence.Append(DOVirtual.Color(_defaultColor, _damageFlashColor, FLASH_COLOR_CHANGE_DURATION, r =>
                {
                    _changedDurabilityImage.color = r;
                }))
                    .Append(DOVirtual.Color(_damageFlashColor, _damagedColor, DAMAGED_COLOR_CHANGE_DURATION, r =>
                    {
                        _changedDurabilityImage.color = r;
                    }))
                    .Append(DOVirtual.Float(1, 0, FADE_DURATION, r => {
                        _damagedColor.a = r;
                        _changedDurabilityImage.color = _damagedColor;
                    }))
                    .Join(DOVirtual.Float(_changedDurabilityImage.fillAmount, _remainingDurabilityImage.fillAmount, FADE_DURATION, r => {
                        _changedDurabilityImage.fillAmount = r;
                    }))
                    .OnComplete(() => {
                        _damagedColor.a = 1;
                        DeleteGetDamagedSequence();
                    });
            }
        }
        private void DeleteGetDamagedSequence()
        {
            DOTween.Kill(_getDamagedSequenceID);
            _getDamagedSequence = null;
        }
        #endregion

        #endregion
    }
}
