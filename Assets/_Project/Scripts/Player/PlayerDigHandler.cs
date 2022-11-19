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
        private const float DIG_DELAY = 0.5f;

        #region PROPERTIES
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
            if (_player.IsInDigZone && !_player.IsDigging && Time.time >= _delayedTime)
            {
                _player.StartedDigging();
                //PlayerEvents.OnStartDigging?.Invoke();
                Debug.Log("started digging");
            }

            CheckForDigIterruption();
        }

        #region PRIVATES
        private void CheckForDigIterruption()
        {
            if (_player.IsFlying) return;

            if (_player.IsDigging && (_player.InputHandler.WalkInput != 0 || _player.InputHandler.FlyInput != 0))
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
            _currentBoxTriggerDirection = triggerDirection;
            _player.EnteredDigZone();
            _delayedTime = Time.time + DIG_DELAY;

            Debug.Log("Side: " + triggerDirection);
        }
        public void StopDiggingProcess()
        {
            //DisablePickaxe();
            _player.ExitedDigZone();
        }
        #endregion

        #region HELPERS
        //private void EnablePickaxe() => pickaxeObj.SetActive(true);
        //private void DisablePickaxe() => pickaxeObj.SetActive(false);
        #endregion
    }
}
