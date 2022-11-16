using UnityEngine;
using ZestCore.Ai;
using DG.Tweening;
using System;

namespace ZestGames
{
    public class AiGetIntoQueue : AiBaseState
    {
        private Ai _ai;

        private QueuePoint _currentQueuePoint;
        private bool _reachedToQueue, _isMoving;

        public QueuePoint CurrentQueuePoint => _currentQueuePoint;

        #region SEQUENCE
        private Sequence _rotationSequence;
        private Guid _rotationSequenceID;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            //Debug.Log("Entered get into queue state.");
            aiStateManager.SwitchStateType(Enums.AiStateType.GetInQueue);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _reachedToQueue = false;
            _currentQueuePoint = QueueManager.ExampleQueue.GetQueue(_ai);

            _ai.OnMove?.Invoke();
            _isMoving = true;
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (_currentQueuePoint == null)
            {
                Debug.Log("Switching to idle, Queue point is null");
                aiStateManager.SwitchState(aiStateManager.IdleState);
                return;
            }

            if (!_reachedToQueue)
            {
                // go to queue
                if (Operation.IsTargetReached(_ai.transform, _currentQueuePoint.transform.position, 0.001f))
                {
                    _reachedToQueue = true;
                    StartRotationSequence();
                }
                else
                {
                    Navigation.MoveTransform(_ai.transform, _currentQueuePoint.transform.position, _ai.WalkSpeed);
                    Navigation.LookAtTarget(_ai.transform, _currentQueuePoint.transform.position);
                }
            }
            else
            {
                // wait for your turn
                if (_isMoving)
                {
                    _ai.OnIdle?.Invoke();
                    _isMoving = false;
                }
            }
        }

        #region PUBLICS
        public void UpdateQueue(QueuePoint queuePoint)
        {
            _currentQueuePoint.QueueIsReleased();
            _currentQueuePoint = queuePoint;
            _currentQueuePoint.QueueIsTaken();

            _reachedToQueue = false;
            _ai.OnMove?.Invoke();
            _isMoving = true;
        }
        public void ActivateStateAfterQueue()
        {
            _ai.StateManager.SwitchState(_ai.StateManager.WalkState);
        }
        #endregion

        private void StartRotationSequence()
        {
            CreateRotationSequence();
            _rotationSequence.Play();
        }

        #region DOTWEEN FUNCTIONS
        private void CreateRotationSequence()
        {
            if (_rotationSequence == null)
            {
                _rotationSequence = DOTween.Sequence();
                _rotationSequenceID = Guid.NewGuid();
                _rotationSequence.id = _rotationSequenceID;

                _rotationSequence.Append(_ai.transform.DORotate(Vector3.zero, 1f)).OnComplete(() => DeleteRotationSequence());
            }
        }
        private void DeleteRotationSequence()
        {
            DOTween.Kill(_rotationSequenceID);
            _rotationSequence = null;
        }
        #endregion
    }
}
