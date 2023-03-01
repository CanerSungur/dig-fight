using UnityEngine;
using DigFight;

namespace ZestGames
{
    public class PlayerCollision : MonoBehaviour
    {
        private Player _player;

        private bool _collidingWithPushableBox, _collidingWithBorderBox;

        public void Init(Player player)
        {
            _player = player;
            _collidingWithBorderBox = _collidingWithPushableBox = false;
        }

        private void CheckIfSquishedBetweenBorderAndPushable()
        {
            if (_collidingWithBorderBox && _collidingWithPushableBox)
            {
                AudioManager.PlayAudio(Enums.AudioType.CharacterPop);
                if (GameManager.PlayerIsRevived)
                {
                    AdEventHandler.OnInterstitialActivateForGameEnd?.Invoke(() => {
                        GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Fail);
                    });
                }
                else
                    GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Fail);

                //AiEvents.OnWin?.Invoke();
                PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.CharacterPop_Confetti, transform.position + new Vector3(0f, 1f, 2f), Quaternion.identity);
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PushableBox pushableBox))
            {
                _collidingWithPushableBox = true;
                CheckIfSquishedBetweenBorderAndPushable();
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("MiddleBox") || collision.gameObject.layer == LayerMask.NameToLayer("BorderBox"))
            {
                _collidingWithBorderBox = true;
                CheckIfSquishedBetweenBorderAndPushable();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PushableBox pushableBox))
                _collidingWithPushableBox = false;

            if (collision.gameObject.layer == LayerMask.NameToLayer("MiddleBox") || collision.gameObject.layer == LayerMask.NameToLayer("BorderBox"))
                _collidingWithBorderBox = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            if (other.TryGetComponent(out UpgradeAreaBase upgradeArea) && !upgradeArea.PlayerIsInArea)
                upgradeArea.StartOpening();

            if (other.TryGetComponent(out QueueActivator queueActivator) && !queueActivator.PlayerIsInArea)
            {
                queueActivator.StartEmptyingQueue(_player);
                //_player.TimerForAction.StartFilling(() => queueActivator.StartEmptyingQueue());
            }

            if (other.TryGetComponent(out ExamplePoint examplePoint) && !examplePoint.PlayerIsInArea)
            {
                examplePoint.PlayerIsInArea = true;
                _player.MoneyHandler.StartSpending(examplePoint);
            }

            #region DIGGING && PUSHING
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                boxDigTrigger.AssignInteracter(_player);

                if (boxDigTrigger.transform.parent.TryGetComponent(out PushableBox pushableBox) && !_player.IsInPushZone)
                {
                    _player.PushHandler.SetPushedBox(pushableBox);
                    _player.PushHandler.StartPushingProcess(boxDigTrigger.TriggerDirection);
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox) && !_player.IsInDigZone)
                {
                    _player.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ExplosiveBox explosiveBox) && !_player.IsInDigZone)
                {
                    _player.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ChestBase chest) && !_player.IsInDigZone && !chest.Triggered)
                {
                    _player.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);
                }
            }
            #endregion
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out UpgradeAreaBase upgradeArea) && upgradeArea.PlayerIsInArea)
                upgradeArea.StopOpening();

            if (other.TryGetComponent(out QueueActivator queueActivator) && queueActivator.PlayerIsInArea)
            {
                queueActivator.StopEmptyingQueue(_player);
                //_player.TimerForAction.StopFilling();
            }

            if (other.TryGetComponent(out ExamplePoint examplePoint) && examplePoint.PlayerIsInArea)
            {
                examplePoint.PlayerIsInArea = false;
                _player.MoneyHandler.StopSpending();
            }

            #region DIGGING && PUSHING
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                if (boxDigTrigger.transform.parent.TryGetComponent(out PushableBox pushableBox) && !_player.IsPushing)
                {
                    _player.StoppedPushing();
                    _player.PushHandler.StopPushingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox))
                {
                    _player.StoppedDigging();
                    _player.DigHandler.StopDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ExplosiveBox explosiveBox))
                {
                    _player.StoppedDigging();
                    _player.DigHandler.StopDiggingProcess();
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ChestBase chest))
                {
                    _player.StoppedDigging();
                    _player.DigHandler.StopDiggingProcess();
                }
            }
            #endregion
        }
    }
}
