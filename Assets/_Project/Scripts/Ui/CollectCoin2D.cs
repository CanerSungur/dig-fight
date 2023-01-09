using DG.Tweening;
using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;
using System;

namespace DigFight
{
    public class CollectCoin2D : MonoBehaviour
    {
        private Camera _camera;
        private RectTransform _canvasRect;

        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;

        #region SEQUENCE
        private Sequence _collectSequence;
        private Guid _collectSequenceID;
        private readonly float _collectDuration = 1f;
        #endregion

        public void Init(MoneyCanvas moneyCanvas, RectTransform rectTransform)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _camera = Camera.main;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
            }

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.one * 0.5f;
            //_rectTransform.anchoredPosition = GetWorldPointToScreenPoint(spawnPosition);
            //_rectTransform.anchoredPosition = spawnPosition;
            _rectTransform.anchoredPosition = SwitchToRectTransform(rectTransform, _rectTransform);

            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            StartCollectSequence();
        }

        private Vector2 GetWorldPointToScreenPoint(Vector3 position)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(position);
            Vector2 phaseUnlockerScreenPosition = new Vector2(
               (viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 1f),
               (viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 1f));

            return phaseUnlockerScreenPosition;
        }
        /// <summary>
        /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        private Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
        {
            Vector2 localPoint;
            Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
            screenP += fromPivotDerivedOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
            Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        #region DOTWEEN FUNCTIONS
        private void StartCollectSequence()
        {
            CreateCollectSequence();
            _collectSequence = null;
        }
        private void CreateCollectSequence()
        {
            if (_collectSequence == null)
            {
                _collectSequence = DOTween.Sequence();
                _collectSequenceID = Guid.NewGuid();
                _collectSequence.id = _collectSequenceID;
                //_rectTransform.DOJumpAnchorPos(Hud.MoneyAnchoredPosition, 2f, 1, 1f);
                //_rectTransform.DOAnchorPos(Hud.MoneyAnchoredPosition, 1f)
                _collectSequence.Append(_rectTransform.DOAnchorPos(_rectTransform.anchoredPosition + new Vector2(Random.Range(-100f, 100f), Random.Range(100f, 200f)), 0.5f))
                    //.Append(DOVirtual.Float(0f, 1f, 0.5f, r => { }))
                    .Append(_rectTransform.DOJumpAnchorPos(Hud.CoinAnchoredPosition, Random.Range(-200, 200), 1, _collectDuration))
                    .Join(_rectTransform.DOScale(Vector3.one, _collectDuration))
                    .Join(_rectTransform.DORotate(Vector3.zero, _collectDuration))
                    .OnComplete(() => {
                        AudioManager.PlayAudio(Enums.AudioType.CollectCoin);
                        CoinEvents.OnCollect?.Invoke(1);
                        DeleteCollectSequence();
                        gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteCollectSequence()
        {
            DOTween.Kill(_collectSequenceID);
            _collectSequence = null;
        }
        #endregion
    }
}
