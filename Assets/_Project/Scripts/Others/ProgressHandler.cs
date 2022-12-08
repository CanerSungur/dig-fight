using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ProgressHandler : MonoBehaviour
    {
        private ProgressLine _progressLine;

        private float _lengthToLevelFinish;
        private int _currentLayer;
        private const float GAP_BETWEEN_LAYERS = 2.25f;

        #region PROPERTIES
        public int CurrentLayer => _currentLayer;
        #endregion

        public void Init(Player player)
        {
            if (_progressLine == null)
            {
                _progressLine = GetComponentInChildren<ProgressLine>();
                _progressLine.Init(this, true);

                _lengthToLevelFinish = GAP_BETWEEN_LAYERS * LayerHandler.TotalLayerCount;
            }
        }

        private void Update()
        {
            UpdatePlayerCurrentLayer();
        }

        private void UpdatePlayerCurrentLayer()
        {
            _lengthToLevelFinish =  10 * GAP_BETWEEN_LAYERS;
            _currentLayer = (int)((Mathf.Abs(transform.position.y) / _lengthToLevelFinish) * 10) + 1;

            if (transform.position.y > 0)
                _currentLayer = 0;
        }
    }
}
