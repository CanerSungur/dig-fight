using UnityEngine;
using ZestCore.Utility;
using ZestGames;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace DigFight
{
    public class AiRunState : AiBaseState
    {
        private Ai _ai;
        private bool _leftIsRunnable, _rightIsRunnable, _movementStarted, _reverseDirection = false;
        private Vector3 _direction;

        #region FALL DELAY
        private float _counter;
        private const float FALL_DELAY = 0.1f;
        #endregion

        #region MAX RUN TIMER
        private float _maxRunTimer;
        private const float MAX_RUN_TIME = 10f;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            Debug.Log("RUN");
            aiStateManager.SwitchStateType(Enums.AiStateType.Run);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _counter = FALL_DELAY;
            _maxRunTimer = MAX_RUN_TIME;
            _movementStarted = false;
            CheckSides();
            if (!_leftIsRunnable && !_rightIsRunnable && !_ai.SurroundingChecker.CanDig && !_ai.SurroundingChecker.CanPush)
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

                if (Time.time > _maxRunTimer)
                    aiStateManager.SwitchState(aiStateManager.IdleState);
            }
            else
            {
                _counter -= Time.deltaTime;
                if (_counter < 0f)
                    aiStateManager.SwitchState(aiStateManager.FallState);
            }
        }

        #region HELPERS
        private void CheckSides()
        {
            _leftIsRunnable = _ai.SurroundingChecker.Left != Enums.Surrounding.Wall;
            _rightIsRunnable = _ai.SurroundingChecker.Right != Enums.Surrounding.Wall;
        }
        private void DecideDirection()
        {
            if (_reverseDirection)
            {
                _direction *= -1f;
                Debug.Log("Reverse Direction!");
                _reverseDirection = false;
                return;
            }

            if (_leftIsRunnable && !_rightIsRunnable)
                _direction = Vector3.left;
            else if (!_leftIsRunnable && _rightIsRunnable)
                _direction = Vector3.right;
            else if (_leftIsRunnable && _rightIsRunnable)
                _direction = RNG.RollDice(50) ? Vector3.left : Vector3.right;
        }
        #endregion

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

        #region PUBLICS
        public void RunToTheOtherSide()
        {
            _reverseDirection = true;
        }
        #endregion
    }
}
