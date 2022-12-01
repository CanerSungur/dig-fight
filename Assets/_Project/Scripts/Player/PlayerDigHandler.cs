using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PlayerDigHandler : MonoBehaviour
    {
        private Player _player;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;

        [Header("-- SETUP --")]
        [SerializeField] private Pickaxe pickaxe;

        private float _delayedTime;
        private const float DIG_DELAY = 0f;

        #region PROPERTIES
        public Player Player => _player;
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
                _player = player;

            pickaxe.Init(this);
            //EnablePickaxe();
        }

        private void Update()
        {
            if ((int)_player.InputHandler.DigDirection == (int)_currentBoxTriggerDirection && _player.IsInDigZone && !_player.IsDigging && !_player.IsPushing && Time.time >= _delayedTime)
            {
                _player.StartedDigging();
                //PlayerEvents.OnStartDigging?.Invoke();
                //Debug.Log("started digging");
            }

            CheckForDigIterruption();
        }

        #region PRIVATES
        private void CheckForDigIterruption()
        {
            if (_player.IsFlying) return;

            if (_player.IsDigging && (int)_player.InputHandler.DigDirection != (int)_currentBoxTriggerDirection)
            {
                _player.StoppedDigging();

                if (_player.IsInDigZone)
                    StartDiggingProcess(_currentBoxTriggerDirection);
                else
                    StopDiggingProcess();
            }
        }
        #endregion

        #region PUBLICS
        public void StartDiggingProcess(Enums.BoxTriggerDirection triggerDirection)
        {
            //EnablePickaxe();
            //_player.PushHandler.StopPushingProcess();

            _currentBoxTriggerDirection = triggerDirection;
            _player.EnteredDigZone();
            _delayedTime = Time.time + DIG_DELAY;

            //Debug.Log("Side: " + triggerDirection);
        }
        public void StopDiggingProcess()
        {
            //DisablePickaxe();
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _player.ExitedDigZone();
        }
        public void ResetTriggerDirection() => _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
        #endregion
    }
}
