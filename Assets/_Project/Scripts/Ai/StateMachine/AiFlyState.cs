using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiFlyState : AiBaseState
    {
        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("FLY");
            aiStateManager.SwitchStateType(Enums.AiStateType.Fly);
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            
        }
    }
}
