using UnityEngine;
using DG.Tweening;
using System;

namespace ZestGames
{
    public class AiIdleState : AiBaseState
    {
        private Ai _ai;

        private bool _canMakeADecision = false;
        private readonly float _decisionDelay = 3f;
        private float _timer;

        #region SEQUENCE
        private Sequence _rotationSequence;
        private Guid _rotationSequenceID;
        private const float ROTATION_DURATION = 1f;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("IDLE");
            aiStateManager.SwitchStateType(Enums.AiStateType.Idle);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _canMakeADecision = false;
            _timer = _decisionDelay;
            AiEvents.OnIdle?.Invoke();

            if (_ai.MeshTransform.localRotation != Quaternion.Euler(0f, 0f, 0f))
                StartRotationSequence();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = _decisionDelay;
                _canMakeADecision = true;
                MakeDecision(aiStateManager);
            }
        }

        private void MakeDecision(AiStateManager aiStateManager)
        {
            if (_canMakeADecision)
            {
                // priority: dig, push, run, fly
                // dice    : 40,  30  , 20,  10
                if (_ai.SurroundingChecker.CanRun)
                    aiStateManager.SwitchState(aiStateManager.RunState);
                else
                    Debug.Log("Cannot run");

                _canMakeADecision = false;
            }
        }

        #region DOTWEEN FUNCTIONS
        private void StartRotationSequence()
        {
            CreateRotationSequence();
            _rotationSequence.Play();
        }
        private void CreateRotationSequence()
        {
            if (_rotationSequence == null)
            {
                _rotationSequence = DOTween.Sequence();
                _rotationSequenceID = Guid.NewGuid();
                _rotationSequence.id = _rotationSequenceID;

                _rotationSequence.Append(_ai.MeshTransform.DOLocalRotate(Vector3.zero, ROTATION_DURATION))
                    .OnComplete(DeleteRotationSequence);
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
