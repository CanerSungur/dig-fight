using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PlayerPushHandler : MonoBehaviour
    {
        private Player _player;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;
        private PushableBox _currentPushedBox = null;

        private float _delayedTime;
        private const float PUSH_DELAY = 0.1f;

        #region PROPERTIES
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        public PushableBox CurrentPushedBox => _currentPushedBox;
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
                _player = player;
        }

        private void Update()
        {
            if ((int)_player.InputHandler.DigDirection == (int)_currentBoxTriggerDirection && _player.IsInPushZone && !_player.IsPushing && !_player.IsDigging && Time.time >= _delayedTime)
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
    }
}
