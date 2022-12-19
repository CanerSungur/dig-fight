using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiFallState : AiBaseState
    {
        private Ai _ai;

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("FALL");
            aiStateManager.SwitchStateType(Enums.AiStateType.Fall);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            AiEvents.OnFall?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (_ai.IsGrounded)
            {
                AiEvents.OnLand?.Invoke();
                aiStateManager.SwitchState(aiStateManager.IdleState);
            }
            else
                Fall();
        }

        private void Fall()
        {
            _ai.Rigidbody.velocity = new Vector3(_ai.Rigidbody.velocity.x, -3.81f, _ai.Rigidbody.velocity.z);
        }
    }
}
