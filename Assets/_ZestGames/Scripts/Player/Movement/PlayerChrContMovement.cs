using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerChrContMovement : MonoBehaviour, IPlayerMovement
    {
        private Player _player;
        private CharacterController _characterController;

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float defaultSpeed = 5f;
        private float _currentSpeed;
        private Vector3 _playerVelocity;
        private const float GRAVITY_VALUE = -9f;
        private bool _startedMoving = false;

        #region PROPERTIES
        public bool IsMoving => _player.InputHandler.InputValue != Vector3.zero;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            _characterController = GetComponent<CharacterController>();
            _currentSpeed = defaultSpeed;

            PlayerEvents.OnSetCurrentMovementSpeed += UpdateMovementSpeed;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            PlayerEvents.OnSetCurrentMovementSpeed -= UpdateMovementSpeed;
        }

        private void Update()
        {
            #region DEFAULT MOVEMENT
            if (_player.IsGrounded && _playerVelocity.y < 0f)
                _playerVelocity.y = 0f;

            Motor();

            if (IsMoving)
            {
                transform.forward = _player.InputHandler.InputValue;

                if (!_startedMoving)
                {
                    PlayerEvents.OnMove?.Invoke();
                    _startedMoving = true;
                }
            }
            else
            {
                if (_startedMoving)
                {
                    PlayerEvents.OnIdle?.Invoke();
                    _startedMoving = false;
                }
            }

            _playerVelocity.y += GRAVITY_VALUE * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);
            #endregion
        }

        public void Motor()
        {
            if (GameManager.GameState == Enums.GameState.Started)
                _characterController.Move(_currentSpeed * Time.deltaTime * _player.InputHandler.InputValue);
        }

        private void UpdateMovementSpeed()
        {
            _currentSpeed = DataManager.MovementSpeed;
        }
    }
}
