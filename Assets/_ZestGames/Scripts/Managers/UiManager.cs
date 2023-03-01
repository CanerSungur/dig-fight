using UnityEngine;
using ZestCore.Utility;
using DigFight;

namespace ZestGames
{
    public class UiManager : MonoBehaviour
    {
        private GameManager _gameManager;

        [Header("-- REFERENCES --")]
        [SerializeField] private TouchToStart touchToStart;
        [SerializeField] private Hud hud;
        [SerializeField] private GameObject levelTextGO;
        [SerializeField] private LevelFail levelFail;
        [SerializeField] private LevelSuccess levelSuccess;
        [SerializeField] private SettingsMinimalUi settings;
        [SerializeField] private UpgradeCanvas upgradeCanvas;
        [SerializeField] private PowerUpCanvas _powerUpCanvas;
        [SerializeField] private ReviveCanvas _reviveCanvas;
        [SerializeField] private ShopCanvas _shopCanvas;
        [SerializeField] private PickaxeUpgradeCanvas _pickaxeUpgradeCanvas;
        [SerializeField] private PickaxeRewardCanvas _pickaxeRewardCanvas;

        [Header("-- UI DELAY SETUP --")]
        [SerializeField, Tooltip("The delay in seconds between the game is won and the win screen is loaded.")]
        private float successScreenDelay = 3.0f;
        [SerializeField, Tooltip("The delay in secods between the game is lost and the fail screen is loaded.")]
        private float failScreenDelay = 3.0f;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            hud.Init(this);
            levelFail.Init(this);
            levelSuccess.Init(this);
            settings.Init(this);
            upgradeCanvas.Init(this);
            _powerUpCanvas.Init(this);
            _reviveCanvas.Init(this);
            _shopCanvas.Init(this);
            _pickaxeUpgradeCanvas.Init(this);
            _pickaxeRewardCanvas.Init(this);

            PlayerUpgradeEvents.OnOpenCanvas?.Invoke();

            touchToStart.gameObject.SetActive(true);
            levelFail.gameObject.SetActive(false);
            levelSuccess.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);
            _reviveCanvas.gameObject.SetActive(false);

            hud.gameObject.SetActive(true);
            levelTextGO.SetActive(false);

            GameEvents.OnGameStart += GameStarted;
            GameEvents.OnGameEnd += GameEnded;

            GameEvents.OnLevelSuccess += HandleLevelSuccess;
            GameEvents.OnLevelFail += HandleLevelFail;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= GameStarted;
            GameEvents.OnGameEnd -= GameEnded;

            GameEvents.OnLevelSuccess -= HandleLevelSuccess;
            GameEvents.OnLevelFail -= HandleLevelFail;
        }

        #region EVENT HANDLER FUNCTIONS
        private void GameStarted()
        {
            touchToStart.gameObject.SetActive(false);
            _shopCanvas.gameObject.SetActive(false);
            settings.gameObject.SetActive(true);
            levelTextGO.SetActive(true);
        }
        private void GameEnded(Enums.GameEnd gameEnd)
        {
            hud.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);

            if (gameEnd == Enums.GameEnd.Fail)
                GameEvents.OnLevelFail?.Invoke();
            else if (gameEnd == Enums.GameEnd.Success)
                GameEvents.OnLevelSuccess?.Invoke();
            else if (gameEnd == Enums.GameEnd.AskForRevive)
            {
                if (GameManager.PlayerIsRevived)
                {
                    AdEventHandler.OnInterstitialActivateForGameEnd?.Invoke(() => {
                        GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Fail);
                    });
                }
                else
                    EnableReviveCanvas();
            }
            else if (gameEnd == Enums.GameEnd.None)
            {
                hud.gameObject.SetActive(true);
                settings.gameObject.SetActive(true);
            }
        }
        private void HandleLevelSuccess() => Delayer.DoActionAfterDelay(this, successScreenDelay, () => levelSuccess.gameObject.SetActive(true));
        private void HandleLevelFail() => Delayer.DoActionAfterDelay(this, failScreenDelay, () => levelFail.gameObject.SetActive(true));
        #endregion

        #region PRIVATES
        private void EnableReviveCanvas()
        {
            _reviveCanvas.gameObject.SetActive(true);
            _reviveCanvas.OpenCanvas();
        }
        #endregion

        #region PUBLICS
        public void OpenShopTab() => _shopCanvas.OpenShopTab();
        public void CloseShopTab() => _shopCanvas.CloseShopTab();
        public void OpenPickaxeUpgradeTab() => _pickaxeUpgradeCanvas.OpenPickaxeUpgradeTab();
        public void ClosePickaxeUpgradeTab() => _pickaxeUpgradeCanvas.ClosePickaxeUpgradeTab();
        #endregion
    }
}
