using DG.Tweening;
using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;
using System;
using TMPro;

namespace DigFight
{
    public class CollectShopCoin2D : MonoBehaviour
    {
        #region COMPONENTS
        private RectTransform _canvasRect;
        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;
        #endregion

        #region SEQUENCE
        private Sequence _collectSequence, _spawnSequence, _goToUiSequence;
        private Guid _collectSequenceID, _spawnSequenceID, _goToUiSequenceID;
        private readonly float _collectDuration = 1f;
        #endregion

        public void Init(MoneyCanvas moneyCanvas, RectTransform spawnRect, RectTransform targetRect)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
            }

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.localScale = Vector3.one;

            //_rectTransform.anchoredPosition = 
            _rectTransform.anchoredPosition = SwitchToRectTransform(_rectTransform, spawnRect);

            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));
            //TriggerSpawnSequence(targetRect);
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
        //private void StartCollectSequence(RectTransform targetRect)
        //{
        //    CreateCollectSequence(targetRect);
        //    _collectSequence = null;
        //}
        //private void CreateCollectSequence(RectTransform targetRect)
        //{
        //    if (_collectSequence == null)
        //    {
        //        _collectSequence = DOTween.Sequence();
        //        _collectSequenceID = Guid.NewGuid();
        //        _collectSequence.id = _collectSequenceID;
        //        //_rectTransform.DOJumpAnchorPos(Hud.MoneyAnchoredPosition, 2f, 1, 1f);
        //        //_rectTransform.DOAnchorPos(Hud.MoneyAnchoredPosition, 1f)
        //        _collectSequence.Append(_rectTransform.DOAnchorPos(_rectTransform.anchoredPosition + new Vector2(Random.Range(-100f, 100f), Random.Range(100f, 200f)), 0.5f))
        //            //.Append(DOVirtual.Float(0f, 1f, 0.5f, r => { }))
        //            .Append(_rectTransform.DOJumpAnchorPos(targetPosition, Random.Range(-200, 200), 1, _collectDuration))
        //            .Join(_rectTransform.DOScale(Vector3.one, _collectDuration))
        //            .Join(_rectTransform.DORotate(Vector3.zero, _collectDuration))
        //            .OnComplete(() => {
        //                AudioManager.PlayAudio(Enums.AudioType.CollectCoin);
        //                CoinEvents.OnCollect?.Invoke(1);
        //                DeleteCollectSequence();
        //                gameObject.SetActive(false);
        //            });
        //    }
        //}
        //private void DeleteCollectSequence()
        //{
        //    DOTween.Kill(_collectSequenceID);
        //    _collectSequence = null;
        //}

        private void TriggerSpawnSequence(RectTransform targetRect)
        {
            CreateSpawnSequence(targetRect);
            _spawnSequence.Play();
        }
        private void CreateSpawnSequence(RectTransform targetRect)
        {
            if (_spawnSequence == null)
            {
                _spawnSequence = DOTween.Sequence();
                _spawnSequenceID = Guid.NewGuid();
                _spawnSequence.id = _spawnSequenceID;

                _spawnSequence.Append(_rectTransform.DOAnchorPos(_rectTransform.anchoredPosition + new Vector2(Random.Range(-100f, 100f), Random.Range(50f, -150f)), 0.5f))
                    .OnComplete(() => {
                        DeleteSpawnSequence();
                        //transform.SetParent(targetRect);
                        //TriggerGoToUiSequence(targetRect);
                    });
            }
        }
        private void DeleteSpawnSequence()
        {
            DOTween.Kill(_spawnSequenceID);
            _spawnSequence = null;
        }

        private void TriggerGoToUiSequence(RectTransform targetRect)
        {
            CreateGoToUiSequence(targetRect);
            _goToUiSequence.Play();
        }

        private void CreateGoToUiSequence(RectTransform targetRect)
        {
            if (_goToUiSequence == null)
            {
                _goToUiSequence = DOTween.Sequence();
                _goToUiSequenceID = Guid.NewGuid();
                _goToUiSequence.id = _goToUiSequenceID;

                _goToUiSequence.Append(_rectTransform.DOJumpAnchorPos(targetRect.anchoredPosition, Random.Range(-200, 200), 1, _collectDuration))
                    .Join(_rectTransform.DOScale(Vector3.one, _collectDuration))
                    .Join(_rectTransform.DORotate(Vector3.zero, _collectDuration))
                    .OnComplete(() => {
                        AudioManager.PlayAudio(Enums.AudioType.CollectCoin);
                        CoinEvents.OnCollect?.Invoke(1);
                        DeleteGoToUiSequence();
                        gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteGoToUiSequence()
        {
            DOTween.Kill(_goToUiSequenceID);
            _goToUiSequence = null;
        }
        #endregion
    }
}
