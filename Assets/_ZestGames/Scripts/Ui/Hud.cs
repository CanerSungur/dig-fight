using UnityEngine;
using TMPro;
using ZestCore.Utility;
using DigFight;

namespace ZestGames
{
    public class Hud : MonoBehaviour
    {
        [Header("-- TEXT --")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI collectableText;
        [SerializeField] private GameObject collectableImage;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameObject coinImage;

        [Header("-- BOOST SETUP --")]
        [SerializeField] private PowerUpIndicator _durabilityPowerUpIndicator;
        [SerializeField] private PowerUpIndicator _speedPowerUpIndicator;
        [SerializeField] private PowerUpIndicator _powerPowerUpIndicator;

        #region STATICS
        public static Transform CollectableHUDTransform { get; private set; }
        public static Vector2 MoneyAnchoredPosition { get; private set; }
        public static Transform CoinHUDTransform { get; private set; }
        public static Vector2 CoinAnchoredPosition { get; private set; }
        #endregion

        public void Init(UiManager uiManager)
        {
            _durabilityPowerUpIndicator.gameObject.SetActive(false);
            _speedPowerUpIndicator.gameObject.SetActive(false);
            _powerPowerUpIndicator.gameObject.SetActive(false);

            UiEvents.OnUpdateLevelText += UpdateLevelText;
            UiEvents.OnUpdateMoneyText += UpdateMoneyText;
            UiEvents.OnUpdateCoinText += UpdateCoinText;

            CollectableHUDTransform = collectableText.transform.parent;
            MoneyAnchoredPosition = collectableImage.GetComponent<RectTransform>().anchoredPosition;
            CoinHUDTransform = coinText.transform.parent;
            CoinAnchoredPosition = coinImage.GetComponent<RectTransform>().anchoredPosition;

            PlayerEvents.OnActivatePickaxeDurability += ActivateDurabilityIndicator;
            PlayerEvents.OnActivatePickaxeSpeed += ActivateSpeedIndicator;
            PlayerEvents.OnActivatePickaxePower += ActivatePowerIndicator;
        }

        private void OnDisable()
        {
            if (GameManager.GameState == Enums.GameState.WaitingToStart) return;

            UiEvents.OnUpdateLevelText -= UpdateLevelText;
            UiEvents.OnUpdateMoneyText -= UpdateMoneyText;
            UiEvents.OnUpdateCoinText -= UpdateCoinText;

            PlayerEvents.OnActivatePickaxeSpeed -= ActivateSpeedIndicator;
        }

        #region EVENT HANDLER FUNCTIONS
        private void UpdateLevelText(int level) => levelText.text = $"Level {level}";
        private void UpdateMoneyText(float money)
        {
            collectableText.text = money.ToString("#0");
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
        private void UpdateCoinText(int coin)
        {
            coinText.text = coin.ToString();
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
        #endregion

        #region POWER UP FUNCTIONS
        private void ActivateDurabilityIndicator(PowerUp powerUp)
        {
            _durabilityPowerUpIndicator.gameObject.SetActive(true);
            _durabilityPowerUpIndicator.Init(this, powerUp);
        }
        private void ActivateSpeedIndicator(PowerUp powerUp)
        {
            _speedPowerUpIndicator.gameObject.SetActive(true);
            _speedPowerUpIndicator.Init(this, powerUp);
        }
        private void ActivatePowerIndicator(PowerUp powerUp)
        {
            _powerPowerUpIndicator.gameObject.SetActive(true);
            _powerPowerUpIndicator.Init(this, powerUp);
        }
        public void DisableDurabilityIndicator() => _durabilityPowerUpIndicator.gameObject.SetActive(false);
        public void DisableSpeedIndicator() => _speedPowerUpIndicator.gameObject.SetActive(false);
        public void DisablePowerIndicator() => _powerPowerUpIndicator.gameObject.SetActive(false);
        #endregion
    }
}
