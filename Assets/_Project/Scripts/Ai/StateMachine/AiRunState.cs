using UnityEngine;
using ZestCore.Utility;
using ZestGames;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace DigFight
{
    public class AiRunState : AiBaseState
    {
        private Ai _ai;
        private bool _leftIsEmpty, _rightIsEmpty, _movementStarted, _onAir = false;
        private Vector3 _direction = Vector3.zero;

        #region FALL DELAY
        private float _counter;
        private const float FALL_DELAY = 0.5f;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("RUN");
            aiStateManager.SwitchStateType(Enums.AiStateType.Run);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _counter = FALL_DELAY;
            _movementStarted = _onAir = false;
            _leftIsEmpty = _ai.SurroundingChecker.Left == Enums.Surrounding.Empty;
            _rightIsEmpty = _ai.SurroundingChecker.Right == Enums.Surrounding.Empty;
            if (!_leftIsEmpty && !_rightIsEmpty)
                aiStateManager.SwitchState(aiStateManager.IdleState);

            DecideDirection();
            AiEvents.OnMove?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            if (_ai.IsGrounded)
            {
                Move(_direction);
                Rotate(_direction);
            }
            else
            {
                _counter -= Time.deltaTime;
                if (_counter < 0f)
                    aiStateManager.SwitchState(aiStateManager.FallState);
            }
        }

        private void DecideDirection()
        {
            if (_leftIsEmpty && !_rightIsEmpty)
                _direction = Vector3.left;
            else if (!_leftIsEmpty && _rightIsEmpty)
                _direction = Vector3.right;
            else if (_leftIsEmpty && _rightIsEmpty)
                _direction = RNG.RollDice(50) ? Vector3.left : Vector3.right;
        }
        private void Move(Vector3 direction)// left or right
        {
            _ai.Rigidbody.velocity = direction * (_ai.CurrentMovementSpeed + _ai.PowerUpHandler.SpeedRate) * Time.deltaTime;
        }
        private void Rotate(Vector3 direction)
        {
            float eulerY;
            if (direction == Vector3.left)
                eulerY = 90f;
            else if (direction == Vector3.right)
                eulerY = -90f;
            else
                eulerY = 0f;

            _ai.MeshTransform.localRotation = Quaternion.Lerp(_ai.MeshTransform.localRotation, Quaternion.Euler(0f, eulerY, 0f), 10f * Time.deltaTime);
        }
    }
}
