using UnityEngine;
using DG.Tweening;
using System;
using ZestGames;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class Piece : MonoBehaviour
    {
        public void Init(PieceHandler pieceHandler)
        {
            transform.localRotation = Quaternion.Euler(Random.Range(0f, 180f), Random.Range(0f, 180f), Random.Range(0f, 180f));
        }

        #region PUBLICS
        public void StartPullOutSequence()
        {
            transform.SetParent(null);
            CreatePullOutSequence();
            _pullOutSequence.Play();
        }
        #endregion

        #region SEQUENCE
        private Sequence _pullOutSequence, _pullInSequence;
        private Guid _pullOutSequenceID, _pullInSequenceID;
        private const float PULL_OUT_DURATION = 1f;
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreatePullOutSequence()
        {
            if (_pullOutSequence == null)
            {
                _pullOutSequence = DOTween.Sequence();
                _pullOutSequenceID = Guid.NewGuid();
                _pullOutSequence.id = _pullOutSequenceID;

                _pullOutSequence.Append(transform.DOShakeScale(PULL_OUT_DURATION * 0.5f, 0.2f))
                    //.Join(transform.DOShakePosition(PULL_OUT_DURATION * 0.5f, 5))
                    .Join(transform.DOMove(transform.position + new Vector3(0f, Random.Range(2f, 3.5f), Random.Range(-2.5f, -1.5f)), PULL_OUT_DURATION))
                    .Join(transform.DORotate(new Vector3(0f, 360f, 0f), PULL_OUT_DURATION, RotateMode.FastBeyond360))
                    .OnComplete(() => {
                        StartPullInSequence();
                        DeletePullOutSequence();
                    });
            }
        }
        private void DeletePullOutSequence()
        {
            DOTween.Kill(_pullOutSequenceID);
            _pullOutSequence = null;
        }
        // ####################
        private void StartPullInSequence()
        {
            CreatePullInSequence();
            _pullInSequence.Play();
        }
        private void CreatePullInSequence()
        {
            if (_pullInSequence == null)
            {
                _pullInSequence = DOTween.Sequence();
                _pullInSequenceID = Guid.NewGuid();
                _pullInSequence.id = _pullInSequenceID;

                transform.SetParent(CharacterTracker.PlayerTransform);
                _pullInSequence.Append(transform.DOLocalJump(Vector3.up, Random.Range(2f, 3f), 1, PULL_OUT_DURATION))
                    //.Join(transform.DOScale(Vector3.zero, PULL_OUT_DURATION))
                    .OnComplete(() => {
                        CollectableEvents.OnSpawnMoney?.Invoke(1, transform.position);
                        DeletePullInSequence();
                        Destroy(gameObject);
                    });
            }
        }
        private void DeletePullInSequence()
        {
            DOTween.Kill(_pullInSequenceID);
            _pullInSequence = null;
        }
        #endregion
    }
}
