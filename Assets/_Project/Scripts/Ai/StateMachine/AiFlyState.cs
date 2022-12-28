using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class AiFlyState : AiBaseState
    {
        private Ai _ai;
        private bool _leftIsEmpty, _rightIsEmpty, _directionDecided;
        private Vector3 _direction = Vector3.zero;

        #region CHECK SIDE TIMER
        private const float CHECK_SIDE_TIMER = 0.5f;
        private float _timer;
        #endregion

        #region MAX RUN TIMER
        private float _maxFlyTimer;
        private const float MAX_FLY_TIME = 10f;
        #endregion

        public override void EnterState(AiStateManager aiStateManager)
        {
            //Debug.Log("FLY");
            aiStateManager.SwitchStateType(Enums.AiStateType.Fly);

            if (_ai == null)
                _ai = aiStateManager.Ai;

            _timer = CHECK_SIDE_TIMER;
            _maxFlyTimer = Time.time + MAX_FLY_TIME;
            _directionDecided = false;
            CheckSides();
            DecideDirection();
            AiEvents.OnFly?.Invoke();
        }

        public override void UpdateState(AiStateManager aiStateManager)
        {
            CheckSides();

            if (_leftIsEmpty || _rightIsEmpty)
            {
                _timer -= Time.deltaTime;
                if (_timer < 0f && (_leftIsEmpty || _rightIsEmpty) && !_directionDecided && RNG.RollDice((_leftIsEmpty && _rightIsEmpty) ? 100 : 50))
                {
                    _timer = CHECK_SIDE_TIMER;
                    _directionDecided = true;
                    DecideDirection();
                }
            }
            else _directionDecided = false;

            Motor(_direction);
            Rotate(_direction);

            if (Time.time > _maxFlyTimer)
                aiStateManager.SwitchState(aiStateManager.FallState);
        }

        private void CheckSides()
        {
            _leftIsEmpty = _ai.SurroundingChecker.Left == Enums.Surrounding.Empty;
            _rightIsEmpty = _ai.SurroundingChecker.Right == Enums.Surrounding.Empty;
        }
        private void DecideDirection()
        {
            if (_leftIsEmpty && !_rightIsEmpty)
                _direction = Vector3.left;
            else if (!_leftIsEmpty && _rightIsEmpty)
                _direction = Vector3.right;
            else if (_leftIsEmpty && _rightIsEmpty)
                _direction = RNG.RollDice(50) ? Vector3.left : Vector3.right;
            else if (!_leftIsEmpty && !_rightIsEmpty)
                _direction = Vector3.zero;
        }

        private void Motor(Vector3 direction)
        {
            _ai.Rigidbody.AddForce(Vector3.up * _ai.CurrentFlySpeed, ForceMode.Force);
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

            _ai.MeshTransform.localRotation = Quaternion.Lerp(_ai.MeshTransform.localRotation, Quaternion.Euler(0f, eulerY, 0f), 5f * Time.deltaTime);
        }
    }
}
