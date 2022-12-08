using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class Debris : MonoBehaviour
    {
        private Vector3 _defaultPosition;

        #region COMPONENTS
        private DebrisContainer _debrisContainer;
        private Rigidbody _rigidbody;
        #endregion

        #region DISPOSE SEQUENCE
        private Sequence _disposeSequence;
        private Guid _disposeSequenceID;
        private const float DISPOSE_DURATION = 3f;
        #endregion

        public void Init(DebrisContainer debrisContainer)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
                _debrisContainer = debrisContainer;
                _defaultPosition = transform.localPosition;
            }

            ResetToDefault();
        }

        private void OnDisable()
        {
            //if (_disposeSequence != null || _disposeSequence.IsPlaying())
            //DeleteDisposeSequence();
            transform.DOKill();
        }

        #region PUBLICS
        public void Release()
        {
            _rigidbody.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 5f), Random.Range(1f, 2f)) * _debrisContainer.ActivaterDebrisHandler.ReleaseForce, ForceMode.Impulse);
            StartDisposeSequence();
        }
        #endregion

        #region PRIVATES
        private void ResetToDefault()
        {
            if (_disposeSequence != null && _disposeSequence.IsPlaying())
                DeleteDisposeSequence();

            transform.localPosition = _defaultPosition;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartDisposeSequence()
        {
            CreateDisposeSequence();
            _disposeSequence.Play();
        }
        private void CreateDisposeSequence()
        {
            if (_disposeSequence == null)
            {
                _disposeSequence = DOTween.Sequence();
                _disposeSequenceID = Guid.NewGuid();
                _disposeSequence.id = _disposeSequenceID;

                _disposeSequence.Append(DOVirtual.Float(0f, 1f, DISPOSE_DURATION, r => { }))
                    .Append(transform.DOScale(0f, DISPOSE_DURATION))
                    .OnComplete(() => {
                        DeleteDisposeSequence();
                        _debrisContainer.Dispose();
                    });
            }
        }
        private void DeleteDisposeSequence()
        {
            DOTween.Kill(_disposeSequenceID);
            _disposeSequence = null;
        }
        #endregion
    }
}
