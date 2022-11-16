using UnityEngine;

namespace ZestGames
{
    public abstract class AiBaseState
    {
        public abstract void EnterState(AiStateManager aiStateManager);
        public abstract void UpdateState(AiStateManager aiStateManager);
    }
}
