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
                InputValue = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
            else
                InputValue = Vector3.zero;
        }
    }
}
