using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class AiDigState : AiBaseState
    {
        private Ai _ai;

        #region SEQUENCE
        private Sequence _rotationSequence;
        private Guid _rotationSequenceID;
        private const float ROTATION_DURATION = 1f;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("DIG");
            aiStateManager.SwitchStateType(Enums.AiStateType.Dig);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            if (_ai.MeshTransform.localRotation != Quaternion.Euler(0f, 0f, 0f))
                StartRotationSequence();

            AiEvents.OnIdle?.Invoke();
            _ai.DigHandler.StartDiggingProcess();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (!_ai.IsGrounded)
                aiStateManager.SwitchState(aiStateManager.FallState);
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
