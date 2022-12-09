using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class PostProcessManager : MonoBehaviour
    {
        private Volume _volume;
        private DepthOfField _depthOfField;

        private const float MIN_FOCAL_LENGTH = 1f;
        private const float MAX_FOCAL_LENGTH = 140f;
        private const float DEPTH_SEQUENCE_DURATION = 2f;

        #region SEQUENCE
        private Sequence _depthSequence;
        private Guid _depthSequenceID;
        #endregion

        public void Init(GameManager gameManager)
        {
            _volume = GetComponent<Volume>();
            _volume.profile.TryGet(out _depthOfField);
        }

        #region PUBLICS
        public void EnableBlur(GameManager gameManager) => _depthOfField.focalLength.value = MAX_FOCAL_LENGTH;
        public void DisableBlur(GameManager gameManager) => StartDepthSequence();
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartDepthSequence()
        {
            CreateDepthSequence();
            _depthSequence.Play();
        }
        private void CreateDepthSequence()
        {
            if (_depthSequence == null)
            {
                _depthSequence = DOTween.Sequence();
                _depthSequenceID = Guid.NewGuid();
                _depthSequence.id = _depthSequenceID;

                _depthSequence.Append(DOVirtual.Float(MAX_FOCAL_LENGTH, MIN_FOCAL_LENGTH, DEPTH_SEQUENCE_DURATION, r => {
                    _depthOfField.focalLength.value = r;
                }))
                    .OnComplete(() => {
                        _depthOfField.focalLength.value = MIN_FOCAL_LENGTH;
                        DeleteDepthSequence();
                    });
            }
        }
        private void DeleteDepthSequence()
        {
            DOTween.Kill(_depthSequenceID);
            _depthSequence = null;
        }
        #endregion
    }
}
