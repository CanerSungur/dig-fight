using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace ZestGames
{
    public class TimerForAction : MonoBehaviour
    {
        private Action _currentAction;

        [Header("-- SETUP --")]
        private Transform _canvas;
        private Image _fillImage;

        #region PROPERTIES
        public bool IsFilling { get; private set; }
        #endregion

        #region SEQUENCE
        private Sequence _fillSequence, _emptySequence;
        private Guid _fillSequenceID, _emptySequenceID;
        #endregion

        public void Init()
        {
            if (_canvas == null)
            {
                _canvas = transform.GetChild(0);
                _fillImage = _canvas.GetChild(0).GetComponent<Image>();
            }
            
            _fillImage.fillAmount = 0;
            _currentAction = null;
            IsFilling = false;
            _canvas.gameObject.SetActive(false);
        }

        private void DoAction()
        {
            _currentAction?.Invoke();
            _currentAction = null;
        }

        #region PUBLICS
        public void StartFilling(float fillTime, Action action)
        {
            IsFilling = true;
            _fillImage.fillAmount = 0;
            _canvas.gameObject.SetActive(true);

            _currentAction += action;
            //PlayerIsInArea = true;

            DeleteEmptySequence();
            CreateFillSequence(fillTime);
            _fillSequence.Play();
        }
        public void StopFilling(float fillTime, Action action)
        {
            DoAction();
            //PlayerIsInArea = false;

            DeleteFillSequence();
            CreateEmptySequence(fillTime);
            _emptySequence.Play();
        }
        public void StopFilling(float fillTime)
        {
            //PlayerIsInArea = false;
            _currentAction = null;

            DeleteFillSequence();
            CreateEmptySequence(fillTime);
            _emptySequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateFillSequence(float fillTime)
        {
            if (_fillSequence == null)
            {
                _fillSequence = DOTween.Sequence();
                _fillSequenceID = Guid.NewGuid();
                _fillSequence.id = _fillSequenceID;

                _fillSequence.Append(_canvas.DOScale(Vector3.one, fillTime * 0.1f))
                    .Append(DOVirtual.Float(_fillImage.fillAmount, 1f, fillTime, r => {
                        _fillImage.fillAmount = r;
                    }))
                    .Append(_canvas.DOScale(Vector3.zero, fillTime * 0.1f)).OnComplete(() => {
                        DeleteFillSequence();
                        DoAction();
                    });
            }
        }
        private void DeleteFillSequence()
        {
            DOTween.Kill(_fillSequenceID);
            _fillSequence = null;
        }

        private void CreateEmptySequence(float fillTime)
        {
            if (_emptySequence == null)
            {
                _emptySequence = DOTween.Sequence();
                _emptySequenceID = Guid.NewGuid();
                _emptySequence.id = _emptySequenceID;

                _emptySequence.Append(DOVirtual.Float(_fillImage.fillAmount, 0f, fillTime, r => {
                    _fillImage.fillAmount = r;
                }))
                    .Append(_canvas.DOScale(Vector3.zero, fillTime * 0.1f)).OnComplete(() => {
                        DeleteEmptySequence();
                        IsFilling = false;
                        _canvas.gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteEmptySequence()
        {
            DOTween.Kill(_emptySequenceID);
            _emptySequence = null;
        }
        #endregion
    }
}
