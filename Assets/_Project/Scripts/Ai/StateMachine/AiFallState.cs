using UnityEngine;
using ZestGames;
using System;

namespace DigFight
{
    public class AiFallState : AiBaseState
    {
        private Ai _ai;
        private bool _hitWallWhileFlying = false;

        #region MAX FALL TIMER
        private const float HIT_WALL_FALL_TIMER = 2f;
        private const float MAX_FALL_TIMER = 10f;
        private float _maxedFallTimer, _hitWallFallTimer;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("FALL");
            aiStateManager.SwitchStateType(Enums.AiStateType.Fall);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _maxedFallTimer = Time.time + MAX_FALL_TIMER;
            _hitWallFallTimer = Time.time + HIT_WALL_FALL_TIMER;
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
            {
                Fall();
                DisableFallAfterMaxTime();
                DisableFallAfterHitWall();
            }
        }

        #region HELPERS
        private void Fall()
        {
            _ai.Rigidbody.velocity = new Vector3(_ai.Rigidbody.velocity.x * 0.9f, -3.81f, _ai.Rigidbody.velocity.z);
        }
        private void DisableFallAfterMaxTime()
        {
            if (Time.time > _maxedFallTimer) _ai.StateManager.SwitchState(_ai.StateManager.FlyState);
        }
        private void DisableFallAfterHitWall()
        {
            if (Time.time > _hitWallFallTimer) _ai.StateManager.SwitchState(_ai.StateManager.FlyState);
        }
        #endregion

        #region PUBLICS
        public void HitWallWhileFlying() => _hitWallWhileFlying = true;
        #endregion
    }
}
