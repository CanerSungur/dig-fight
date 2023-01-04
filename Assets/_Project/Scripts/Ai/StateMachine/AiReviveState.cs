using UnityEngine;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class AiReviveState : AiBaseState
    {
        private Ai _ai;

        private bool _reviveFinished;
        private Sequence _reviveSequence;
        private Guid _reviveSequenceID;

        public override void EnterState(AiStateManager aiStateManager)
        {
            aiStateManager.SwitchStateType(Enums.AiStateType.Revive);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _reviveFinished = false;
            StartReviveSequence();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (_reviveFinished)
                aiStateManager.SwitchState(aiStateManager.FallState);
        }

        #region REVIVE SEQUENCE
        private void StartReviveSequence()
        {
            CreateReviveSequence();
            _reviveSequence.Play();
        }
        private void CreateReviveSequence()
        {
            if (_reviveSequence == null)
            {
                _reviveSequence = DOTween.Sequence();
                _reviveSequenceID = Guid.NewGuid();
                _reviveSequence.id = _reviveSequenceID;

                _ai.Rigidbody.useGravity = false;

                _reviveSequence.Append(_ai.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce))
                    .Append(_ai.transform.DOMove(_ai.StartingPosition, 0.5f))
                    .Append(_ai.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce))
                    .OnComplete(() => {
                        _ai.transform.localScale = Vector3.one;
                        _ai.transform.position = _ai.StartingPosition;
                        _reviveFinished = _ai.Rigidbody.useGravity = true;
                        DeleteReviveSequence();
                    });
            }
        }
        private void DeleteReviveSequence()
        {
            DOTween.Kill(_reviveSequenceID);
            _reviveSequence = null;
        }
        #endregion
    }
}
