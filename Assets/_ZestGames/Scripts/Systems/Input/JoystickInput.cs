using UnityEngine;

namespace ZestGames
{
    public class JoystickInput : MonoBehaviour
    {
        private Player _player;

        [Header("-- INPUT SETUP --")]
        [SerializeField] private Joystick joystick;

        public Vector3 InputValue { get; private set; }
        public bool CanTakeInput => GameManager.GameState == Enums.GameState.Started && Time.time >= _delayedTime && !_player.IsUpgrading;

        // first input delay
        private float _delayedTime;
        private readonly float _delayRate = 1f;

        private float _verticalInput;

        #region PROPERTIES
        public bool IsMovingUp => _verticalInput > 0.1f;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            GameEvents.OnGameStart += () => _delayedTime = Time.time + _delayRate;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            GameEvents.OnGameStart -= () => _delayedTime = Time.time + _delayRate;
        }

        private void Update()
        {
            if (CanTakeInput)
            {
                _verticalInput = joystick.Vertical;
                if (_verticalInput <= 0.1f)
                    _verticalInput = 0f;

                InputValue = new Vector3(joystick.Horizontal, _verticalInput, 0f);
            }
            else
                InputValue = Vector3.zero;
        }
    }
}
