using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class AppearOnEnable : MonoBehaviour
    {
        private Transform _meshTransform;

        private const float MIN_Y_POSITION = -1.5f;
        private const float MAX_Y_POSITION = -0.5f;

        #region SEQUENCE
        private Sequence _appearSequence;
        private Guid _appearSequenceID;
        private const float MIN_APPEAR_DURATION = 0.75f;
        private const float MAX_APPEAR_DURATION = 1.1f;
        private float _currentAppearDuration;
        #endregion

        private void OnEnable()
        {
            _meshTransform = transform.GetChild(0);
            _meshTransform.localPosition = new Vector3(0f, Random.Range(MIN_Y_POSITION, MAX_Y_POSITION), 0f);
            _currentAppearDuration = Random.Range(MIN_APPEAR_DURATION, MAX_APPEAR_DURATION);

            StartAppearSequence();
        }

        private void StartAppearSequence()
        {
            CreateAppearSequence();
            _appearSequence.Play();
        }
        private void CreateAppearSequence()
        {
            if (_appearSequence == null)
            {
                _appearSequence = DOTween.Sequence();
                _appearSequenceID = Guid.NewGuid();
                _appearSequence.id = _appearSequenceID;

                _appearSequence.Append(_meshTransform.DOLocalMoveY(0f, _currentAppearDuration))
                    .Append(_meshTransform.DOShakeScale(.75f, .5f))
                    .OnComplete(DeleteAppearSequence);
            }
        }
        private void DeleteAppearSequence()
        {
            DOTween.Kill(_appearSequenceID);
            _appearSequence = null;
        }
    }
}
