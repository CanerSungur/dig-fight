using UnityEngine;
using ZestCore.Ai;

namespace ZestGames
{
    public class AiWalkState : AiBaseState
    {
        private Ai _ai;

        private Vector3 _randomPosition;
        private bool _targetReached = false;

        public override void EnterState(AiStateManager aiStateMachine)
        {
            //Debug.Log("Entered walk state");
            aiStateMachine.SwitchStateType(Enums.AiStateType.WalkRandom);

            if (_ai == null)
                _ai = aiStateMachine.Ai;

            _targetReached = false;
            _randomPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

            _ai.OnMove?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (_ai == null) return;

            if (!_targetReached)
            {
                if (Operation.IsTargetReached(_ai.transform, _randomPosition, 0.5f))
                {
                    _targetReached = true;
                    aiStateManager.SwitchState(aiStateManager.IdleState);
                }
                else
                {
                    Navigation.MoveTransform(_ai.transform, _randomPosition, _ai.WalkSpeed);
                    Navigation.LookAtTarget(_ai.transform, _randomPosition);
                }
            }
        }
    }
}
