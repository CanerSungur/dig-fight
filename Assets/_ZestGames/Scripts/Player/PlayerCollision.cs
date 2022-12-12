using UnityEngine;
using DigFight;

namespace ZestGames
{
    public class PlayerCollision : MonoBehaviour
    {
        private Player _player;

        public void Init(Player player)
        {
            _player = player;
        }

        private void OnTriggerEnter(Collider other)
        {
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

            #region DIGGING
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger))
            {
                if (boxDigTrigger.transform.parent.TryGetComponent(out PushableBox pushableBox) && !_player.IsInPushZone)
                {
                    boxDigTrigger.AssignInteracter(_player);
                    _player.PushHandler.SetPushedBox(pushableBox);
                    _player.PushHandler.StartPushingProcess(boxDigTrigger.TriggerDirection);
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out BreakableBox breakableBox) && !_player.IsInDigZone)
                {
                    boxDigTrigger.AssignInteracter(_player);
                    _player.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);
                }
                else if (boxDigTrigger.transform.parent.TryGetComponent(out ExplosiveBox explosiveBox) && !_player.IsInDigZone)
                {
                    boxDigTrigger.AssignInteracter(_player);
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

            #region DIGGING
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
            }
            #endregion
        }
    }
}
