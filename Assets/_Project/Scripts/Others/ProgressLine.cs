using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using ZestGames;

namespace DigFight
{
    public class ProgressLine : MonoBehaviour
    {
        //private Transform _targetTransform;
        private ProgressHandler _progressHandler;

        #region FIXED CANVAS POSITION
        private bool _isItPlayer = false;
        private const float FIXED_X_POSITION_FOR_PLAYER = -1.7f;
        private const float FIXED_X_POSITION_FOR_AI = 11f;
        #endregion

        #region LINE TO TARGET
        private LineRenderer _lineRenderer;
        private const float LINE_RENDERER_OFFSET = 0.7f;
        #endregion

        #region TEXT OF REMAINING BOXES TO FINISH
        private TextMeshProUGUI _remainingBoxesText;
        #endregion

        #region DISABLE SEQUENCE
        private Sequence _disableSequence;
        private Guid _disableSequenceID;
        #endregion

        public void Init(ProgressHandler progressHandler, bool isItPlayer)
        {
            if (_progressHandler == null)
            {
                _progressHandler = progressHandler;
                _lineRenderer = GetComponentInChildren<LineRenderer>();
                _remainingBoxesText = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            _isItPlayer = isItPlayer;

            GameEvents.OnGameEnd += StartDisableSequence;
        }

        private void OnDisable()
        {
            if (_progressHandler == null) return;
            GameEvents.OnGameEnd -= StartDisableSequence;
        }

        private void LateUpdate()
        {
            FixPositionX();
            SetLineRendererTargetPosition();
            UpdateRemainingBoxText();
        }

        private void FixPositionX()
        {
            transform.position = _isItPlayer == true ?
                new Vector3(FIXED_X_POSITION_FOR_PLAYER, transform.position.y, transform.position.z) :
                new Vector3(FIXED_X_POSITION_FOR_AI, transform.position.y, transform.position.z);
        }
        private void SetLineRendererTargetPosition()
        {
            if (_progressHandler.transform.position.x > 0)
                _lineRenderer.SetPosition(1, new Vector3(_progressHandler.transform.position.x + LINE_RENDERER_OFFSET, 0f, 0f));
            else
                _lineRenderer.SetPosition(1, Vector3.zero);
        }
        private void UpdateRemainingBoxText()
        {
            _remainingBoxesText.text = (LayerHandler.TotalLayerCount - _progressHandler.CurrentLayer - 1).ToString();
        }

        #region DOTWEEN FUNCTIONS
        private void StartDisableSequence(Enums.GameEnd ignoreThis)
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

                _disableSequence.Append(transform.DOScale(Vector3.zero, 1f)).SetEase(Ease.InBounce)
                    .OnComplete(DeleteDisableSequence);
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
