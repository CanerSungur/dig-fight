using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiLoseState : AiBaseState
    {
        public override void EnterState(AiStateManager aiStateManager)
        {
            AiEvents.OnLose?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            
        }
    }
}
