using UnityEngine;

namespace ZestGames
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Player _player;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        // Booleans
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _groundedID = Animator.StringToHash("Grounded");
        private readonly int _flyingID = Animator.StringToHash("Flying");
        private readonly int _tooHighID = Animator.StringToHash("TooHigh");
        private readonly int _diggingID = Animator.StringToHash("Digging");

        // Triggers
        private readonly int _dieID = Animator.StringToHash("Die");
        private readonly int _winID = Animator.StringToHash("Win");
        private readonly int _loseID = Animator.StringToHash("Lose");
        private readonly int _cheerID = Animator.StringToHash("Cheer");

        // Integers
        private readonly int _cheerIndexID = Animator.StringToHash("CheerIndex");
        private readonly int _digSideIndexID = Animator.StringToHash("DigSideIndex");
        #endregion

        #region DIG SIDE DATA
        private const int DIG_TOP_SIDE_INDEX = 0;
        private const int DIG_LEFT_SIDE_INDEX = 1;
        private const int DIG_RIGHT_SIDE_INDEX = 2;
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
            {
                _player = player;
                _animator = GetComponent<Animator>();
            }

            Land();

            PlayerEvents.OnMove += Move;
            PlayerEvents.OnIdle += Idle;
            PlayerEvents.OnDie += Die;
            PlayerEvents.OnWin += Win;
            PlayerEvents.OnLose += Lose;
            PlayerEvents.OnCheer += Cheer;
            PlayerEvents.OnFly += Fly;
            PlayerEvents.OnFall += Fall;
            PlayerEvents.OnLand += Land;
            PlayerEvents.OnStartDigging += StartDigging;
            PlayerEvents.OnStopDigging += StopDigging;
        }

        private void OnDisable()
        {
            if (_player == null) return;

            PlayerEvents.OnMove -= Move;
            PlayerEvents.OnIdle -= Idle;
            PlayerEvents.OnDie -= Die;
            PlayerEvents.OnWin -= Win;
            PlayerEvents.OnLose -= Lose;
            PlayerEvents.OnCheer -= Cheer;
            PlayerEvents.OnFly -= Fly;
            PlayerEvents.OnFall -= Fall;
            PlayerEvents.OnLand -= Land;
            PlayerEvents.OnStartDigging -= StartDigging;
            PlayerEvents.OnStopDigging -= StopDigging;
        }

        #region BASIC ANIM FUNCTIONS
        private void Idle() => _animator.SetBool(_moveID, false);
        private void Move() => _animator.SetBool(_moveID, true);
        private void Die() => _animator.SetTrigger(_dieID);
        private void Win() => _animator.SetTrigger(_winID);
        private void Lose() => _animator.SetTrigger(_loseID);
        private void SelectRandomCheer() => _animator.SetInteger(_cheerIndexID, Random.Range(1, 5));
        private void Cheer()
        {
            SelectRandomCheer();
            _animator.SetTrigger(_cheerID);
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void Fly()
        {
            _animator.SetBool(_groundedID, false);
            _animator.SetBool(_flyingID, true);
        }
        private void Fall()
        {
            CheckForHeight();
            _animator.SetBool(_groundedID, false);
            _animator.SetBool(_flyingID, false);
        }
        private void Land()
        {
            _animator.SetBool(_groundedID, true);
        }
        private void StartDigging()
        {
            _animator.SetInteger(_digSideIndexID, (int)_player.DigHandler.CurrentBoxTriggerDirection);
            _animator.SetBool(_diggingID, true);
        }
        private void StopDigging()
        {
            _animator.SetBool(_diggingID, false);
        }
        #endregion

        #region HELPERS
        private void CheckForHeight()
        {
            _animator.SetBool(_tooHighID, _player.IsTooHigh);
        }
        #endregion

        #region ANIMATION EVENT FUNCTIONS
        public void AlertObservers(string message)
        {
            if (message.Equals("DigMotionEnded"))
            {
                _player.StoppedDigging();
                _player.DigHandler.StartDiggingProcess(_player.DigHandler.CurrentBoxTriggerDirection);
            }
        }
        #endregion
    }
}
