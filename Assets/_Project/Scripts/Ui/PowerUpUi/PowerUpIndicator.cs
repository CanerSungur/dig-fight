using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace DigFight
{
    public class PowerUpIndicator : MonoBehaviour
    {
        #region COMPONENTS
        private Hud _hud;
        private Image _fillImage;
        private PowerUp _currentPowerUp;
        #endregion

        #region SEQUENCE
        private Sequence _enableSequence, _countdownSequence, _disableSequence;
        private Guid _enableSequenceID, _countdownSequenceID, _disableSequenceID;
        #endregion

        public void Init(Hud hud, PowerUp powerUp)
        {
            if (_hud == null)
            {
                _hud = hud;
                _fillImage = GetComponent<Image>();
                _currentPowerUp = powerUp;
            }

            transform.localScale = Vector3.zero;

            StartEnableSequence();
            StartCountdownSequence();
        }

        #region ENABLE SEQUENCE
        private void StartEnableSequence()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                transform.localScale = Vector3.one;
                _enableSequence.Append(transform.DOShakeScale(1f, 0.5f))
                    .OnComplete(() => {
                        DeleteEnableSequence();
                        //StartCountdownSequence();
                    });
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        #endregion

        #region COUNTDOWN SEQUENCE
        private void StartCountdownSequence()
        {
            CreateCountdownSequence();
            _countdownSequence.Play();
        }
        private void CreateCountdownSequence()
        {
            if (_countdownSequence == null)
            {
                _countdownSequence = DOTween.Sequence();
                _countdownSequenceID = Guid.NewGuid();
                _countdownSequence.id = _countdownSequenceID;
                if (_currentPowerUp.Name == "DURABILITY")
                {
                    _fillImage.fillAmount = 0f;
                    _countdownSequence.Append(transform.DOScale(Vector3.one * 1.1f, 1f))
                        .Append(transform.DOScale(Vector3.one, 1f))
                        .Append(transform.DOScale(Vector3.one * 0.9f, 1f))
                        .Append(transform.DOScale(Vector3.one, 1f))
                        .OnComplete(() => {
                            DeleteCountdownSequence();
                            StartDisableSequence();
                        });
                }
                else
                {
                    _countdownSequence.Append(DOVirtual.Float(1f, 0f, _currentPowerUp.Duration, r => {
                        _fillImage.fillAmount = r;
                    }))
                    .OnComplete(() => {
                        DeleteCountdownSequence();
                        StartDisableSequence();
                    });
                }
            }
        }
        private void DeleteCountdownSequence()
        {
            DOTween.Kill(_countdownSequenceID);
            _countdownSequence = null;
        }
        #endregion

        #region DISABLE SEQUENCE
        private void StartDisableSequence()
        {
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

                _disableSequence.Append(transform.DOShakeScale(1f, 0.5f))
                    .Append(transform.DOScale(Vector3.zero, 0.5f))
                    .OnComplete(() => {
                        DeleteDisableSequence();

                        if (_currentPowerUp.Name == "SPEED")
                            _hud.DisableSpeedIndicator();
                        else if (_currentPowerUp.Name == "POWER")
                            _hud.DisablePowerIndicator();
                        else if (_currentPowerUp.Name == "DURABILITY")
                            _hud.DisableDurabilityIndicator();
                    });
            }
        }
        private void DeleteDisableSequence()
        {
            DOTween.Kill(_disableSequenceID);
            _disableSequence = null;
        }
        #endregion
    }
}
