using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PlayerPushHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float _pushSpeed = 2.5f;

        private Player _player;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;
        private PushableBox _currentPushedBox = null;

        private float _delayedTime;
        private const float PUSH_DELAY = 0.5f;

        #region PROPERTIES
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        public PushableBox CurrentPushedBox => _currentPushedBox;
        #endregion

        #region STATICS
        public static float PushDuration { get; private set; }
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
                _player = player;

            PushDuration = _pushSpeed;

            PlayerEvents.OnSetCurrentPickaxeSpeed += UpdatePushSpeed;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            PlayerEvents.OnSetCurrentPickaxeSpeed -= UpdatePushSpeed;
        }

        private void Update()
        {
            if ((int)_player.InputHandler.DigDirection == (int)_currentBoxTriggerDirection && _player.IsInPushZone && !_player.IsPushing && !_player.IsDigging && Time.time >= _delayedTime && _currentPushedBox.IsReadyForPushing)
            {
                _player.StartedPushing();
            }
        }

        #region PUBLICS
        public void StartPushingProcess(Enums.BoxTriggerDirection triggerDirection)
        {
            //_player.DigHandler.StopDiggingProcess();

            //EnablePickaxe();
            _currentBoxTriggerDirection = triggerDirection;
            _player.EnteredPushZone();
            _delayedTime = Time.time + PUSH_DELAY;

            //Debug.Log("Side: " + triggerDirection);
        }
        public void StopPushingProcess()
        {
            //DisablePickaxe();
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _player.ExitedPushZone();
        }
        public void SetPushedBox(PushableBox box) => _currentPushedBox = box;
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void UpdatePushSpeed()
        {
            PushDuration = _pushSpeed - (_pushSpeed * _player.PowerUpHandler.SpeedRate);
            if (PushDuration < 1f)
                PushDuration = 1f;
        }
        #endregion
    }
}
