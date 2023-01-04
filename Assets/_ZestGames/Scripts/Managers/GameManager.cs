using UnityEngine;
using DG.Tweening;
using DigFight;

namespace ZestGames
{
    public class GameManager : MonoBehaviour
    {
        #region STATICS
        public static Enums.GameState GameState { get; private set; }
        public static Enums.GameEnd GameEnd { get; private set; }
        public static bool PlayerIsRevived { get; private set; }
        #endregion

        [SerializeField] private float _gameTime = 1f;

        [Header("-- REFERENCES --")]
        private UiManager _uiManager;
        private LevelManager _levelManager;
        private SettingsManager _settingsManager;
        private DataManager _dataManager;
        //private QueueManager _queueManager;
        //private BoxSpawnManager _boxSpawnManager;
        private MoneySpawnManager _moneySpawnManager;
        private HapticManager _hapticManager;
        private AdEventHandler _adEventHandler;
        [SerializeField] private PostProcessManager _postProcessManager;

        #region PROPERTIES
        //public BoxSpawnManager BoxSpawnManager => _boxSpawnManager;
        public SettingsManager SettingsManager => _settingsManager;
        #endregion

        private void Init()
        {
            Application.targetFrameRate = 240;
            DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(500, 125);

            PlayerIsRevived = false;
            GameState = Enums.GameState.WaitingToStart;
            GameEnd = Enums.GameEnd.None;

            _levelManager = GetComponent<LevelManager>();
            _levelManager.Init(this);
            _dataManager = GetComponent<DataManager>();
            _dataManager.Init(this);
            _settingsManager = GetComponent<SettingsManager>();
            _settingsManager.Init(this);
            _uiManager = GetComponent<UiManager>();
            _uiManager.Init(this);
            //_queueManager = GetComponent<QueueManager>();
            //_queueManager.Init(this);
            //_boxSpawnManager = GetComponent<BoxSpawnManager>();
            //_boxSpawnManager.Init(this);
            _moneySpawnManager = GetComponent<MoneySpawnManager>();
            _moneySpawnManager.Init(this);
            _hapticManager = GetComponent<HapticManager>();
            _hapticManager.Init(this);
            _adEventHandler = GetComponent<AdEventHandler>();
            _adEventHandler.Init(this);
            //_postProcessManager.Init(this);

            UiEvents.OnUpdateCollectableText?.Invoke(DataManager.TotalMoney);
            UiEvents.OnUpdateCoinText?.Invoke(DataManager.TotalCoin);
            UiEvents.OnUpdateLevelText?.Invoke(LevelHandler.Level);

            //_postProcessManager.EnableBlur(this);
        }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameEnd += HandleGameEnd;
            PlayerEvents.OnRevive += HandlePlayerRevive;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameEnd -= HandleGameEnd;
            PlayerEvents.OnRevive -= HandlePlayerRevive;

            DOTween.KillAll();
        }

        private void Update()
        {
            //Time.timeScale = _gameTime;
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandleGameStart()
        {
            GameState = Enums.GameState.Started;
            //_postProcessManager.DisableBlur(this);
        }
        private void HandlePlayerRevive()
        {
            GameState = Enums.GameState.Started;
            PlayerIsRevived = true;
        }
        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            GameEnd = gameEnd;

            if (gameEnd == Enums.GameEnd.Success)
            {
                GameEvents.OnLevelSuccess?.Invoke();
                GameState = Enums.GameState.GameEnded;
            }
            else if (gameEnd == Enums.GameEnd.Fail)
            {
                GameEvents.OnLevelFail?.Invoke();
                GameState = Enums.GameState.GameEnded;
            }
            else if (gameEnd == Enums.GameEnd.AskForRevive)
                GameState = Enums.GameState.GameEnded;
        }
        #endregion
    }
}
