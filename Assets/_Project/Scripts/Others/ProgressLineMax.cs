using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using ZestGames;

namespace DigFight
{
    public class ProgressLineMax : MonoBehaviour
    {
        #region COMPONENTS
        private ProgressManager _progressManager;
        private CanvasGroup _canvasGroup;
        private TextMeshProUGUI _maxDepthAchievedText;
        #endregion

        #region INIT SEQUENCE
        private Sequence _initSequence;
        private Guid _initSequenceID;
        private const float INIT_DURATION = 3f;
        #endregion

        public void Init(ProgressManager progressManager)
        {
            if (_progressManager == null)
            {
                _progressManager = progressManager;
                _canvasGroup = GetComponent<CanvasGroup>();
                _maxDepthAchievedText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            if (_progressManager.MaxDepthAchieved == 0)
                StopInitialize();
            else
                StartInitSequence();

            GameEvents.OnGameEnd += HandleGameEnd;
        }

        private void OnDisable()
        {
            if (_progressManager == null) return;
            GameEvents.OnGameEnd -= HandleGameEnd;
        }

        private void StopInitialize()
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;
        }
        private void SetMaxDepthAchievedText() => _maxDepthAchievedText.text = _progressManager.MaxDepthAchieved.ToString("#.#") + " ft.";

        #region EVENT HANDLER FUNCTIONS
        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            StopInitialize();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartInitSequence()
        {
            SetMaxDepthAchievedText();
            CreateInitSequence();
            _initSequence.Play();
        }
        private void CreateInitSequence()
        {
            if (_initSequence == null)
            {
                _initSequence = DOTween.Sequence();
                _initSequenceID = Guid.NewGuid();
                _initSequence.id = _initSequenceID;

                _initSequence.Append(DOVirtual.Float(0f, 1f, 1f, r => { _canvasGroup.alpha = r; }))
                    .Join(transform.DOShakeScale(1f, 0.005f))
                    .Join(transform.DOLocalMoveY(-_progressManager.MaxDepthAchieved, INIT_DURATION))
                    .OnComplete(() => {
                        _canvasGroup.alpha = 1f;
                        DeleteInitSequence();
                    });
            }
        }
        private void DeleteInitSequence()
        {
            DOTween.Kill(_initSequenceID);
            _initSequence = null;
        }
        #endregion
    }
}
