using System.Collections;
using UnityEngine;

namespace ZestGames
{
    public class QueueActivator : MonoBehaviour
    {
        private QueueSystem _queueSystem;
        private readonly float _queueActivatorDelay = 1f;
        private readonly WaitForSeconds _waitForBetweenActivations = new WaitForSeconds(1.5f);
        private IEnumerator _emptyCoroutine;

        #region PROPERTIES
        public bool PlayerIsInArea { get; private set; }
        #endregion

        public void Init(QueueSystem queueSystem)
        {
            if (_queueSystem == null)
                _queueSystem = queueSystem;

            PlayerIsInArea = false;
            _emptyCoroutine = null;
        }

        #region PUBLICS
        public void StartEmptyingQueue(Player player)
        {
            PlayerIsInArea = true;

            if (_queueSystem.EmptyQueuePoints.Count == _queueSystem.Capacity) // queue is empty
                Debug.Log("Line is empty");
            else
                StartEmptyingCoroutine(player);
        }
        public void StopEmptyingQueue(Player player)
        {
            PlayerIsInArea = false;
            player.TimerForAction.StopFilling(_queueActivatorDelay);
            StopEmptyingCoroutine();
        }
        #endregion

        #region COROUTINE FUNCTIONS
        private void StartEmptyingCoroutine(Player player)
        {
            _emptyCoroutine = EmptyQueueCoroutine(player);
            StartCoroutine(_emptyCoroutine);
        }
        private void StopEmptyingCoroutine() => StopCoroutine(_emptyCoroutine);
        private IEnumerator EmptyQueueCoroutine(Player player)
        {
            while (_queueSystem.EmptyQueuePoints.Count < _queueSystem.Capacity)
            {
                player.TimerForAction.StartFilling(_queueActivatorDelay, () => PlayerEvents.OnEmptyNextInQueue?.Invoke());
                yield return _waitForBetweenActivations;
            }
        }
        #endregion
    }
}
