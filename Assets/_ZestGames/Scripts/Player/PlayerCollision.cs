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
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger) && !_player.IsInDigZone)
            {
                _player.DigHandler.StartDiggingProcess(boxDigTrigger.TriggerDirection);

                //if (boxDigTrigger.TriggerDirection == Enums.BoxTriggerDirection.Top)
                //    _player.DigHandler.StartDiggingProcess(Enums.BoxTriggerDirection.Top);
                //else if (boxDigTrigger.TriggerDirection == Enums.BoxTriggerDirection.Left)
                //    _player.DigHandler.StartDiggingProcess(Enums.BoxTriggerDirection.Left);
                //else if (boxDigTrigger.TriggerDirection == Enums.BoxTriggerDirection.Right)
                //    _player.DigHandler.StartDiggingProcess(Enums.BoxTriggerDirection.Right);
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
            if (other.TryGetComponent(out BoxDigTrigger boxDigTrigger) && _player.IsInDigZone)
            {
                _player.StoppedDigging();
                _player.DigHandler.StopDiggingProcess();
            }
            #endregion
        }
    }
}
