using UnityEngine;
using DigFight;
using System;

namespace ZestGames
{
    public class AiStateManager : MonoBehaviour
    {
        private Ai _ai;
        private Enums.AiStateType _currentStateType;
        private AiBaseState _currentState;

        #region STATES
        public AiIdleState IdleState = new AiIdleState();
        public AiFallState FallState = new AiFallState();
        public AiRunState RunState = new AiRunState();
        public AiFlyState FlyState = new AiFlyState();
        public AiDigState DigState = new AiDigState();
        public AiPushState PushState = new AiPushState();
        public AiWinState WinState = new AiWinState();
        public AiLoseState LoseState = new AiLoseState();
        #endregion

        #region PROPERTIES
        public Ai Ai => _ai;
        public Enums.AiStateType CurrentStateType => _currentStateType;
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            GameEvents.OnGameEnd += HandleGameEnd;

            _currentState = IdleState;
            _currentState.EnterState(this);
        }

        private void OnDisable()
        {
            if (_ai == null) return;

            GameEvents.OnGameEnd -= HandleGameEnd;
        }

        private void FixedUpdate()
        {
            if (_ai == null) return;
            _currentState.UpdateState(this);
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            if (gameEnd == Enums.GameEnd.Fail)
                SwitchState(WinState);
            else if (gameEnd == Enums.GameEnd.Success)
                SwitchState(LoseState);
        }
        #endregion

        #region PUBLICS
        public void SwitchState(AiBaseState state, Action action = null)
        {
            _currentState = state;
            state.EnterState(this);

            action?.Invoke();
        }
        public void SwitchStateType(Enums.AiStateType stateType)
        {
            _currentStateType = stateType;
        }
        #endregion
    }
}
