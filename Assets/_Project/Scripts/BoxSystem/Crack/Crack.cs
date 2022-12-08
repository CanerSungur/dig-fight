using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using System;

namespace DigFight
{
    public class Crack : MonoBehaviour
    {
        private CrackHandler _crackHandler;
        private Transform _meshTransform;

        #region SEQUENCE
        private Sequence _enhanceSequence;
        private Guid _enhanceSequenceID;
        #endregion

        public void Init(CrackHandler crackHandler)
        {
            if (_crackHandler == null)
            {
                _crackHandler = crackHandler;
                _meshTransform = transform.GetChild(0);
            }

            SetRandomRotation();
            transform.localScale = Vector3.zero;
        }

        #region PUBLICS
        public void Enhance(float scaleRate)
        {
            StartEnhanceSequence(scaleRate);
        }
        public void Dispose()
        {
            if (_enhanceSequence != null) DeleteEnhanceSequence();
            Destroy(gameObject);
        }
        #endregion

        private void SetRandomRotation() => _meshTransform.localRotation = Quaternion.Euler(0f, Random.Range(0, 180), 0f);

        #region DOTWEEN FUNCTIONS
        private void StartEnhanceSequence(float scaleRate)
        {
            DeleteEnhanceSequence();
            CreateEnhanceSequence(scaleRate);
            _enhanceSequence.Play();
        }
        private void CreateEnhanceSequence(float scaleRate)
        {
            if (_enhanceSequence == null)
            {
                _enhanceSequence = DOTween.Sequence();
                _enhanceSequenceID = Guid.NewGuid();
                _enhanceSequence.id = _enhanceSequenceID;

                _enhanceSequence.Append(transform.DOScale(scaleRate, 0.1f))
                    .Append(transform.DOShakeScale(0.5f, 0.5f))
                    .OnComplete(() => {
                        transform.localScale = Vector3.one * scaleRate;
                        DeleteEnhanceSequence();
                    });
            }
        }
        private void DeleteEnhanceSequence()
        {
            DOTween.Kill(_enhanceSequenceID);
            _enhanceSequence = null;
        }
        #endregion
    }
}
