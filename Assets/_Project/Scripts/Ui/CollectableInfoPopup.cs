using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using ZestGames;
using DG.Tweening.Core;
using UnityEngine.UI;

namespace DigFight
{
    public class CollectableInfoPopup : MonoBehaviour
    {
        #region SEQUENCE
        private Sequence _collectSequence, _spendSequence;
        private Guid _collectSequenceID, _spendSequenceID;

        private const float COLLECT_SPEND_OFFSET = 75f;
        private const float SEQUENCE_DURATION = 1f;
        #endregion

        #region COMPONENTS
        private RectTransform _rectTransform;
        private Image _infoImage;
        private TextMeshProUGUI _infoText;
        private CanvasGroup _canvasGroup;
        #endregion

        #region HELPERS
        private void CacheComponents()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
                _infoImage = transform.GetChild(0).GetComponent<Image>();
                _infoText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
        private void SetSprite(Sprite sprite) => _infoImage.sprite = sprite;
        #endregion

        #region PUBLICS
        public void TriggerCollectSequence(Sprite collectableSprite, int collectAmount, Action actionAfterSequence)
        {
            CacheComponents();
            SetSprite(collectableSprite);

            CreateCollectSequence(collectAmount, actionAfterSequence);
            _collectSequence.Play();
        }
        public void TriggerSpendSequence(Sprite collectableSprite, int spendAmount)
        {
            CacheComponents();
            SetSprite(collectableSprite);

            CreateSpendSequence(spendAmount);
            _spendSequence.Play();
        }
        #endregion


        #region DOTWEEN FUNCTIONS
        private void CreateCollectSequence(int collectAmount, Action action)
        {
            if (_collectSequence == null)
            {
                _collectSequence = DOTween.Sequence();
                _collectSequenceID = Guid.NewGuid();
                _collectSequence.id = _collectSequenceID;

                _rectTransform.anchoredPosition = Vector2.down * COLLECT_SPEND_OFFSET;
                _infoText.text = "+" + collectAmount;

                _collectSequence.Append(DOVirtual.Float(0f, 1f, SEQUENCE_DURATION * 0.5f, r => {
                    _canvasGroup.alpha = r;
                }))
                    .Join(_rectTransform.DOAnchorPos(Vector2.zero, SEQUENCE_DURATION))
                    .Join(DOVirtual.Color(Color.white, Color.green, SEQUENCE_DURATION, r => {
                        _infoText.color = r;
                    }))
                    .OnComplete(() => {
                        DeleteCollectSequence();

                        _infoText.color = Color.white;
                        action?.Invoke();
                        Destroy(gameObject);
                    });
            }
        }
        private void DeleteCollectSequence()
        {
            DOTween.Kill(_collectSequenceID);
            _collectSequence = null;
        }
        // ##############################
        private void CreateSpendSequence(int spendAmount)
        {
            if (_spendSequence == null)
            {
                _spendSequence = DOTween.Sequence();
                _spendSequenceID = Guid.NewGuid();
                _spendSequence.id = _spendSequenceID;

                _rectTransform.anchoredPosition = Vector2.zero;
                _infoText.text = "-" + spendAmount;

                _spendSequence.Append(_rectTransform.DOAnchorPos(_rectTransform.anchoredPosition + (Vector2.down * COLLECT_SPEND_OFFSET), SEQUENCE_DURATION))
                    
                    .Join(DOVirtual.Color(Color.white, Color.red, SEQUENCE_DURATION, r => {
                        _infoText.color = r;
                    }))
                    .Append(DOVirtual.Float(1f, 0f, SEQUENCE_DURATION * 0.25f, r => {
                        _canvasGroup.alpha = r;
                    }))
                    .OnComplete(() => {
                        DeleteSpendSequence();

                        _infoText.color = Color.white;
                        Destroy(gameObject);
                    });
            }
        }
        private void DeleteSpendSequence()
        {
            DOTween.Kill(_spendSequenceID);
            _spendSequence = null;
        }
        #endregion
    }
}
