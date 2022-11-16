using UnityEngine;
using TMPro;

namespace ZestGames
{
    public class UpgradeCanvasItem : MonoBehaviour
    {
        private UpgradeCanvas _upgradeCanvas;

        public TextMeshProUGUI LevelText { get; private set; }
        public TextMeshProUGUI CostText { get; private set; }
        public CustomButton Button { get; private set; }

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
                LevelText = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                CostText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
                Button = transform.GetChild(0).GetComponent<CustomButton>();
            }
        }
    }
}
