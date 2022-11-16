using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public abstract class QueueSystem : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private int capacity = 5;
        [SerializeField] private QueuePoint queuePointPrefab;

        private readonly Quaternion _defaultRotation = Quaternion.Euler(90f, 0f, 0f);
        private readonly float _queueOffset = -0.75f;
        private bool _updatingQueue = false;

        #region PRIVATES
        private QueueActivator _queueActivator;
        #endregion

        #region PROPERTIES
        public bool QueueIsFull => EmptyQueuePoints.Count == 0 && !_updatingQueue;
        public int Capacity => capacity;
        #endregion

        #region QUEUE POINTS LIST SYSTEM
        private List<QueuePoint> _emptyQueuePoints;
        public List<QueuePoint> EmptyQueuePoints => _emptyQueuePoints == null ? _emptyQueuePoints = new List<QueuePoint>() : _emptyQueuePoints;
        public void AddQueuePoint(QueuePoint queuePoint)
        {
            if (!EmptyQueuePoints.Contains(queuePoint))
                EmptyQueuePoints.Add(queuePoint);
        }
        public void RemoveQueuePoint(QueuePoint queuePoint)
        {
            if (EmptyQueuePoints.Contains(queuePoint))
                EmptyQueuePoints.Remove(queuePoint);
        }
        #endregion

        #region AIS IN QUEUE LIST
        private List<Ai> _aisInQueue;
        public List<Ai> AisInQueue => _aisInQueue == null ? _aisInQueue = new List<Ai>() : _aisInQueue;
        public void AddAiInQueue(Ai ai)
        {
            if (!AisInQueue.Contains(ai))
                AisInQueue.Add(ai);
        }
        public void RemoveAiFromQueue(Ai ai)
        {
            if (AisInQueue.Contains(ai))
                AisInQueue.Remove(ai);
        }
        #endregion

        private void Start()
        {
            _queueActivator = GetComponentInChildren<QueueActivator>();
            _queueActivator.Init(this);

            SpawnQueuePoints(capacity);

            PlayerEvents.OnEmptyNextInQueue += UpdateQueue;
        }

        private void OnDisable()
        {
            PlayerEvents.OnEmptyNextInQueue -= UpdateQueue;
        }

        private void SpawnQueuePoints(int count)
        {
            int queue = 0;
            for (int i = 0; i < count; i++)
            {
                QueuePoint queuePoint = Instantiate(queuePointPrefab, Vector3.zero, _defaultRotation, transform);
                queuePoint.Init(this, i + 1);
                queuePoint.transform.localPosition = new Vector3(0f, 0f, queue * _queueOffset);
                AddQueuePoint(queuePoint);
                queue++;
            }
        }

        #region PUBLICS
        public QueuePoint GetQueue(Ai ai)
        {
            AddAiInQueue(ai);
            QueuePoint queue = EmptyQueuePoints[0];
            queue.QueueIsTaken();
            return queue;
        }
        public QueuePoint GetNextQueue(QueuePoint queuePoint)
        {
            QueuePoint nextQueue = EmptyQueuePoints[EmptyQueuePoints.IndexOf(queuePoint) - 1];
            queuePoint.QueueIsReleased();
            nextQueue.QueueIsTaken();
            return nextQueue;
        }
        public void UpdateQueue()
        {
            _updatingQueue = true;

            Ai firstAi = AisInQueue[0];
            RemoveAiFromQueue(firstAi);
            firstAi.StateManager.GetIntoQueueState.CurrentQueuePoint.QueueIsReleased();
            firstAi.StateManager.GetIntoQueueState.ActivateStateAfterQueue();

            for (int i = 0; i < AisInQueue.Count; i++)
            {
                Ai ai = AisInQueue[i];
                //ai.StateManager.GetIntoQueueState.CurrentQueuePoint.QueueIsReleased();
                ai.StateManager.GetIntoQueueState.UpdateQueue(EmptyQueuePoints[0]);
                // go to next queue
            }

            _updatingQueue = false;
        }
        #endregion
    }
}
