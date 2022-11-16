using UnityEngine;
using TMPro;
using ZestCore.Utility;

namespace ZestGames
{
    public class Hud : MonoBehaviour
    {
        [Header("-- TEXT --")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI collectableText;
        [SerializeField] private GameObject collectableImage;

        public static Transform CollectableHUDTransform { get; private set; }
        public static Vector2 MoneyAnchoredPosition { get; private set; }

        public void Init(UiManager uiManager)
        {
            UiEvents.OnUpdateLevelText += UpdateLevelText;
            UiEvents.OnUpdateCollectableText += UpdateMoneyText;

            CollectableHUDTransform = collectableText.transform.parent;
            MoneyAnchoredPosition = collectableImage.GetComponent<RectTransform>().anchoredPosition;
        }

        private void OnDisable()
        {
            if (GameManager.GameState == Enums.GameState.WaitingToStart) return;

            UiEvents.OnUpdateLevelText -= UpdateLevelText;
            UiEvents.OnUpdateCollectableText -= UpdateMoneyText;
        }

        private void UpdateLevelText(int level) => levelText.text = $"Level {level}";
        private void UpdateMoneyText(float money)
        {
            collectableText.text = money.ToString("#0");
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
    }
}
