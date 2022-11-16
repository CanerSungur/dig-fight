using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRigidKinematicMovement : MonoBehaviour, IPlayerMovement
    {
        private Player _player;

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float defaultSpeed = 5f;
        private float _currentSpeed;

        #region PROPERTIES
        public bool IsMoving => _player && _player.Rigidbody.velocity.magnitude > 0.1f;
        #endregion

        #region INTERFACE FUNCTIONS
        public void Init(Player player)
        {
            if (_player == null)
            {
                _player = player;
            }

            _currentSpeed = defaultSpeed;
        }
        public void Motor()
        {
            if (GameManager.GameState == Enums.GameState.Started)
                _player.Rigidbody.MovePosition(transform.position + _player.InputHandler.InputValue * Time.deltaTime * _currentSpeed);
        }
        #endregion

        private void FixedUpdate()
        {
            Motor();

            if (!_player.IsGrounded && !_player.InputHandler.IsMovingUp)
                EnableGravity();

            if (_player.IsGrounded || _player.InputHandler.IsMovingUp)
                DisableGravity();
        }

        #region HELPERS
        private void EnableGravity()
        {
            _player.Rigidbody.isKinematic = false;
        }
        private void DisableGravity()
        {
            _player.Rigidbody.isKinematic = true;
            _player.Rigidbody.velocity = _player.Rigidbody.angularVelocity = Vector3.zero;
        }
        #endregion
    }
}
