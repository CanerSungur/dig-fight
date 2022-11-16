using UnityEngine;

namespace ZestGames
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Player _player;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _dieID = Animator.StringToHash("Die");
        private readonly int _winID = Animator.StringToHash("Win");
        private readonly int _loseID = Animator.StringToHash("Lose");
        private readonly int _cheerID = Animator.StringToHash("Cheer");
        private readonly int _cheerIndexID = Animator.StringToHash("CheerIndex");
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
            {
                _player = player;
                _animator = GetComponent<Animator>();
            }

            PlayerEvents.OnMove += Move;
            PlayerEvents.OnIdle += Idle;
            PlayerEvents.OnDie += Die;
            PlayerEvents.OnWin += Win;
            PlayerEvents.OnLose += Lose;
            PlayerEvents.OnCheer += Cheer;
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
    }
}
