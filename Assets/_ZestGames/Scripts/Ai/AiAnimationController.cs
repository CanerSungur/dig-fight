using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(Animator))]
    public class AiAnimationController : MonoBehaviour
    {
        private Ai _ai;
        private Animator _animator;

        #region ANIM PARAMETER SETUP
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _dieID = Animator.StringToHash("Die");
        private readonly int _winID = Animator.StringToHash("Win");
        private readonly int _loseID = Animator.StringToHash("Lose");
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
            {
                _ai = ai;
                _animator = GetComponent<Animator>();
            }

            _ai.OnIdle += Idle;
            _ai.OnMove += Move;
            _ai.OnDie += Die;
            _ai.OnWin += Win;
            _ai.OnLose += Lose;
        }

        private void OnDisable()
        {
            if (_ai == null) return;

            _ai.OnIdle -= Idle;
            _ai.OnMove -= Move;
            _ai.OnDie -= Die;
            _ai.OnWin -= Win;
            _ai.OnLose -= Lose;
        }

        #region BASIC ANIM FUNCTIONS
        private void Idle() => _animator.SetBool(_moveID, false);
        private void Move() => _animator.SetBool(_moveID, true);
        private void Die() => _animator.SetTrigger(_dieID);
        private void Win() => _animator.SetTrigger(_winID);
        private void Lose() => _animator.SetTrigger(_loseID);
        #endregion
    }
}
