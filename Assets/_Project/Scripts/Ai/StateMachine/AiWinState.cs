using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiWinState : AiBaseState
    {
        public override void EnterState(AiStateManager aiStateManager)
        {
            AiEvents.OnWin?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            
        }
    }
}
