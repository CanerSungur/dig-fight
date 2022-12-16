using UnityEngine;

namespace ZestGames
{
    public class JoystickInput : MonoBehaviour
    {
        private Player _player;

        [Header("-- INPUT SETUP --")]
        [SerializeField] private Joystick joystick;

        // first input delay
        private float _delayedTime;
        private readonly float _delayRate = 1f;

        private const float FLY_INPUT_THRESHOLD = 0.2f;
        private bool _highFall = false;

        #region PROPERTIES
        public bool IsMovingUp => joystick.Vertical > FLY_INPUT_THRESHOLD;
        public Vector3 InputValue { get; private set; }
        public float FlyInput { get; private set; }
        public float WalkInput { get; private set; }
        public bool CanTakeInput => GameManager.GameState == Enums.GameState.Started && Time.time >= _delayedTime && !_player.IsUpgrading && !_player.IsPushing && !_player.IsDigging;
        public Enums.MovementDirection MovementDirection { get; private set; }
        public Enums.DigDirection DigDirection { get; private set; }
        #endregion

        public void Init(Player player)
        {
            _player = player;
            DigDirection = Enums.DigDirection.None;

            GameEvents.OnGameStart += () => _delayedTime = Time.time + _delayRate;
            PlayerEvents.OnFall += CheckForHighFall;
            PlayerEvents.OnLand += CheckForStagger;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            GameEvents.OnGameStart -= () => _delayedTime = Time.time + _delayRate;
            PlayerEvents.OnFall -= CheckForHighFall;
            PlayerEvents.OnLand -= CheckForStagger;
        }

        private void Update()
        {
            SetMovementDirection();
            SetDigDirection();
            //Debug.Log(DigDirection);

            if (CanTakeInput)
            {
                TakeFlyInput();

                WalkInput = joystick.Horizontal;

                InputValue = new Vector3(joystick.Horizontal, 0f, 0f).normalized;
            }
            else
            {
                InputValue = Vector3.zero;
                FlyInput = WalkInput = 0f;
            }
        }

        #region PRIVATES
        private void TakeFlyInput()
        {
            FlyInput = joystick.Vertical;
            if (FlyInput <= FLY_INPUT_THRESHOLD)
                FlyInput = 0f;
        }
        private void SetMovementDirection()
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            if (joystick.Horizontal == 0)
                MovementDirection = Enums.MovementDirection.None;
            else
                MovementDirection = joystick.Horizontal > 0 ? Enums.MovementDirection.Left : Enums.MovementDirection.Right;
        }
        private void SetDigDirection()
        {
            if (joystick.Horizontal >= .9f)
                DigDirection = Enums.DigDirection.Right;
            else if (joystick.Horizontal <= -.9f)
                DigDirection = Enums.DigDirection.Left;
            else if (joystick.Vertical >= .9f)
                DigDirection = Enums.DigDirection.Up;
            else if (joystick.Vertical <= -.9f)
                DigDirection = Enums.DigDirection.Down;
            else
                DigDirection = Enums.DigDirection.None;
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void CheckForHighFall()
        {
            _highFall = _player.IsTooHigh;
        }
        private void CheckForStagger()
        {
            if (_highFall)
                _delayedTime = Time.time + _delayRate;
        }
        #endregion
    }
}
