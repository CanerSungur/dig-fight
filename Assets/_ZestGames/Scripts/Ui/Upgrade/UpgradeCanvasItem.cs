using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace ZestGames
{
    public class UpgradeCanvasItem : MonoBehaviour
    {
        private UpgradeCanvas _upgradeCanvas;

        public TextMeshProUGUI LevelText { get; private set; }
        public TextMeshProUGUI CostText { get; private set; }
        public CustomButton Button { get; private set; }

        #region LEVEL SHAKE SEQUENCE
        private Sequence _shakeLevelImageSequence;
        private Guid _shakeLevelImageSequenceID;
        private const float SHAKE_DURATION = 0.5f;
        private Transform _levelImage;
        #endregion

        public void Init(UpgradeCanvas upgradeCanvas)
        {
            _upgradeCanvas = upgradeCanvas;

            if (_upgradeCanvas.CurrentType == UpgradeCanvas.Type.Idle)
            {
                LevelText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                CostText = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                Button = transform.GetChild(0).GetComponent<CustomButton>();
            }
            else if (_upgradeCanvas.CurrentType == UpgradeCanvas.Type.Incremental)
            {
                LevelText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
                CostText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
                Button = transform.GetChild(1).GetComponent<CustomButton>();
                _levelImage = transform.GetChild(0);
            }
        }
        public void ShakeLevelImage() => StartShakeLevelImageSequence();

        #region DOTWEEN FUNCTIONS
        private void StartShakeLevelImageSequence()
        {
            DeleteShakeLevelImageSequence();
            CreateShakeLevelImageSequence();
            _shakeLevelImageSequence.Play();
        }
        private void CreateShakeLevelImageSequence()
        {
            if (_shakeLevelImageSequence == null)
            {
                _shakeLevelImageSequence = DOTween.Sequence();
                _shakeLevelImageSequenceID = Guid.NewGuid();
                _shakeLevelImageSequence.id = _shakeLevelImageSequenceID;

                _shakeLevelImageSequence.Append(_levelImage.DOShakeScale(SHAKE_DURATION, .5f))
                    .OnComplete(DeleteShakeLevelImageSequence);
            }
        }
        private void DeleteShakeLevelImageSequence()
        {
            DOTween.Kill(_shakeLevelImageSequenceID);
            _shakeLevelImageSequence = null;
        }
        #endregion
    }
}
