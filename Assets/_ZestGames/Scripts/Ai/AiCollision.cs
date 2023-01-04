using DigFight;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace ZestGames
{
    public class AiCollision : MonoBehaviour
    {
        private Ai _ai;

        private const int BOX_DIG_CHANCE = 30;
        private const int EXPLOSIVE_DIG_CHANCE = 70;
        private const int CHEST_DIG_CHANCE = 70;
        private const int PUSH_CHANCE = 40;

        private bool _collidingWithPushableBox, _collidingWithBorderBox;

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            _collidingWithBorderBox = _collidingWithPushableBox = false;
        }

        private void CheckIfSquishedBetweenBorderAndPushable()
        {
            if (_collidingWithBorderBox && _collidingWithPushableBox)
            {
                AudioManager.PlayAudio(Enums.AudioType.CharacterPop);
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Success);
                //PlayerEvents.OnWin?.Invoke();
                PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.CharacterPop_Confetti, transform.position + new Vector3(0f, 1f, 2f), Quaternion.identity);
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BorderBox") || collision.gameObject.layer == LayerMask.NameToLayer("MiddleBox"))
            {
                if (_ai.StateManager.CurrentStateType == Enums.AiStateType.Run)
                    _ai.StateManager.SwitchState(_ai.StateManager.IdleState);
                else if (_ai.StateManager.CurrentStateType == Enums.AiStateType.Fly)
                    _ai.StateManager.SwitchState(_ai.StateManager.FallState, _ai.StateManager.FallState.HitWallWhileFlying);
            }

            if (collision.transform.TryGetComponent(out PushableBox pushableBox))
            {
                _collidingWithPushableBox = true;
                CheckIfSquishedBetweenBorderAndPushable();
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("BorderBox") || collision.gameObject.layer == LayerMask.NameToLayer("MiddleBox"))
            {
                _collidingWithBorderBox = true;
                CheckIfSquishedBetweenBorderAndPushable();
            }

            if (collision.gameObject.TryGetComponent(out LevelEndBox levelEndBox) && GameManager.GameState == Enums.GameState.Started)
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.AskForRevive);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.TryGetComponent(out PushableBox pushableBox))
                _collidingWithPushableBox = false;

            if (collision.gameObject.layer == LayerMask.NameToLayer("BorderBox") || collision.gameObject.layer == LayerMask.NameToLayer("MiddleBox"))
                _collidingWithBorderBox = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                if (_ai.StateManager.CurrentStateType == Enums.AiStateType.Run && (boxDigTrigger.TriggerDirection == Enums.BoxTriggerDirection.Left || boxDigTrigger.TriggerDirection == Enums.BoxTriggerDirection.Right))
                {
                    Debug.Log("Running to Idle...");
                    _ai.StateManager.SwitchState(_ai.StateManager.IdleState, _ai.StateManager.IdleState.ReverseRunDirection);
                }

                boxDigTrigger.AssignInteracter(_ai);

                if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox) && !_ai.IsInDigZone)
                {
                    _ai.DigHandler.AssignCurrentTriggerDirection(boxDigTrigger.TriggerDirection);
                    _ai.DigHandler.StartDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ExplosiveBox explosiveBox) && !_ai.IsInDigZone)
                {
                    _ai.DigHandler.AssignCurrentTriggerDirection(boxDigTrigger.TriggerDirection);
                    _ai.DigHandler.StartDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ChestBase chest) && !_ai.IsInDigZone && !chest.Triggered)
                {
                    _ai.DigHandler.AssignCurrentTriggerDirection(boxDigTrigger.TriggerDirection);
                    _ai.DigHandler.StartDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out PushableBox pushableBox) && !_ai.IsInPushZone)
                {
                    _ai.PushHandler.SetPushedBox(pushableBox);
                    _ai.PushHandler.AssignCurrentTriggerDirection(boxDigTrigger.TriggerDirection);
                    _ai.PushHandler.StartPushingProcess();
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
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ExplosiveBox explosiveBox))
                {
                    _ai.StoppedDigging();
                    _ai.DigHandler.StopDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ChestBase chest))
                {
                    _ai.StoppedDigging();
                    _ai.DigHandler.StopDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out PushableBox pushableBox) && !_ai.IsPushing)
                {
                    _ai.StoppedPushing();
                    _ai.PushHandler.StopPushingProcess();
                }
            }
        }
    }
}
