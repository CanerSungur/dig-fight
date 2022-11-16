using UnityEngine;

namespace ZestGames
{
    public class AiStateManager : MonoBehaviour
    {
        private Ai _ai;
        private Enums.AiStateType _currentStateType;
        private AiBaseState _currentState;

        #region STATES
        public AiIdleState IdleState = new AiIdleState();
        public AiWalkState WalkState = new AiWalkState();
        public AiGetIntoQueue GetIntoQueueState = new AiGetIntoQueue();
        #endregion

        #region PROPERTIES
        public Ai Ai => _ai;
        public Enums.AiStateType CurrentStateType => _currentStateType;
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            _currentState = IdleState;
            _currentState.EnterState(this);
        }

        private void Update()
        {
            if (_ai == null) return;
            _currentState.UpdateState(this);
        }

        #region PUBLICS
        public void SwitchState(AiBaseState state)
        {
            _currentState = state;
            state.EnterState(this);
        }
        public void SwitchStateType(Enums.AiStateType stateType)
        {
            _currentStateType = stateType;
        }
        #endregion
    }
}
