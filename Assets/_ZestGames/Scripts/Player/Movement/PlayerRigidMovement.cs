using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRigidMovement : MonoBehaviour, IPlayerMovement
    {
        private Player _player;

        [Header("-- WALK SETUP --")]
        [SerializeField] private float defaultSpeed = 150f;
        private float _currentSpeed, _accelerationTimer, _accelerationTimeElapsed;
        private const float ACCELERATION_DURATION = 1f;
        private bool _movementStarted = false;

        [Header("-- FLY SETUP --")]
        [SerializeField] private float flySpeed = 100f;
        private bool _flyStarted, _onAir = false;

        #region PROPERTIES
        public bool IsMoving => _player && _player.Rigidbody.velocity.magnitude > 0.05f;
        #endregion

        #region INTERFACE FUNCTIONS
        public void Init(Player player)
        {
            if (_player == null)
                _player = player;

            _accelerationTimeElapsed = 0f;
            _accelerationTimer = ACCELERATION_DURATION;
            _currentSpeed = defaultSpeed;
        }

        public void Motor()
        {
            if (GameManager.GameState == Enums.GameState.Started)
            {
                if (Input.GetMouseButton(0))
                    AccelerateWalkSpeed();
                if (Input.GetMouseButtonUp(0))
                    ResetAccelerationWalk();

                _player.Rigidbody.velocity = new Vector3(_player.InputHandler.WalkInput, 0, 0) * (_currentSpeed + _player.PowerUpHandler.SpeedRate) * Time.fixedDeltaTime;

                Fall();
            }
        }
        #endregion

        private void FixedUpdate()
        {
            if (_player.IsGrounded)
                HandleOnGroundStates();
            else
                HandleOnAirStates();

            Motor();
            Fly();
        }

        #region PRIVATES
        private void Fly()
        {
            if (GameManager.GameState == Enums.GameState.Started && _player.CanFly)
            {
                _player.Rigidbody.AddForce(new Vector3(0f, _player.InputHandler.FlyInput * flySpeed, 0f), ForceMode.Force);
            }

            Fall();
        }
        private void Fall()
        {
            if (!_player.IsGrounded && _player.InputHandler.FlyInput == 0f) // Falling
            {
                //_player.Rigidbody.AddForce(new Vector3(0f, -9.8f, 0f), ForceMode.Force);
                _player.Rigidbody.velocity = new Vector3(_player.Rigidbody.velocity.x, -3.81f, _player.Rigidbody.velocity.z);
            }
        }
        private void AccelerateWalkSpeed()
        {
            if (_accelerationTimeElapsed < ACCELERATION_DURATION)
            {
                _currentSpeed = Mathf.Lerp(defaultSpeed * 0.5f, defaultSpeed, _accelerationTimeElapsed / ACCELERATION_DURATION);
                _accelerationTimeElapsed += Time.fixedDeltaTime;
            }
        }
        private void ResetAccelerationWalk()
        {
            _accelerationTimeElapsed = _currentSpeed = 0f;
        }
        private void HandleOnGroundStates()
        {
            if (IsMoving && !_movementStarted)
            {
                _movementStarted = true;
                PlayerEvents.OnMove?.Invoke();
            }
            if (!IsMoving && _movementStarted)
            {
                _movementStarted = false;
                PlayerEvents.OnIdle?.Invoke();
            }

            if (_player.IsGrounded && _onAir)
            {
                _onAir = false;
                PlayerEvents.OnLand?.Invoke();
            }
        }
        private void HandleOnAirStates()
        {
            if (_player.CanFly && !_flyStarted)
            {
                _flyStarted = _onAir = true;
                PlayerEvents.OnFly?.Invoke();
            }
            if (!_player.CanFly && _flyStarted)
            {
                _flyStarted = false;
                PlayerEvents.OnFall?.Invoke();
            }

            if (!_onAir)
            {
                _onAir = true;
                PlayerEvents.OnFall?.Invoke();
            }
        }
        #endregion
    }
}
