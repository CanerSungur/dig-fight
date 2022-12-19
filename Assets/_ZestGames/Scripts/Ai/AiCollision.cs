using DigFight;
using UnityEngine;
using ZestCore.Utility;

namespace ZestGames
{
    public class AiCollision : MonoBehaviour
    {
        private Ai _ai;

        private const int DIG_CHANCE = 10;

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_ai.StateManager.CurrentStateType == Enums.AiStateType.Run && collision.gameObject.layer == LayerMask.NameToLayer("BorderBox"))
                _ai.StateManager.SwitchState(_ai.StateManager.IdleState);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                boxDigTrigger.AssignInteracter(_ai);

                if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox) && !_ai.IsInDigZone && RNG.RollDice(DIG_CHANCE))
                {
                    _ai.DigHandler.AssignCurrentTriggerDirection(boxDigTrigger.TriggerDirection);
                    _ai.StateManager.SwitchState(_ai.StateManager.DigState);
                    //_ai.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox))
                {
                    _ai.StoppedDigging();
                    _ai.DigHandler.StopDiggingProcess();
                }
            }
        }
    }
}
