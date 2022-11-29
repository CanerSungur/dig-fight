using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class PlayerRotationHandler : MonoBehaviour
    {
        private Player _player;
        private Transform _meshTransform;

        #region ROTATION DATA
        private float _targetRotation;
        private const float RESET_ROTATION = 0f;
        private const float LEFT_ROTATION = -90f;
        private const float RIGHT_ROTATION = 90f;
        private const float ROTATION_DURATION = 5f;
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
                _player = player;

            _meshTransform = transform.GetChild(0);
        }

        private void Update()
        {
            SetMeshTargetRotation();
            UpdateMeshRotation();
        }

        #region PRIVATES
        private void SetMeshTargetRotation()
        {
            //if (_player.InputHandler.MovementDirection == Enums.MovementDirection.None)
            //    _targetRotation = RESET_ROTATION;
            //else if (_player.InputHandler.MovementDirection == Enums.MovementDirection.Left)
            //    _targetRotation = LEFT_ROTATION;
            //else if (_player.InputHandler.MovementDirection == Enums.MovementDirection.Right)
            //    _targetRotation = RIGHT_ROTATION;

            if (_player.InputHandler.MovementDirection == Enums.MovementDirection.Left)
                _targetRotation = Mathf.Abs(_player.InputHandler.WalkInput) * LEFT_ROTATION;
            else if (_player.InputHandler.MovementDirection == Enums.MovementDirection.Right)
                _targetRotation = Mathf.Abs(_player.InputHandler.WalkInput) * RIGHT_ROTATION;
            else if (_player.InputHandler.MovementDirection == Enums.MovementDirection.None)
                _targetRotation = RESET_ROTATION;
        }
        private void UpdateMeshRotation() => _meshTransform.localRotation = Quaternion.Lerp(_meshTransform.localRotation, Quaternion.Euler(0f, _targetRotation, 0f), ROTATION_DURATION * Time.deltaTime);
        #endregion
    }
}
