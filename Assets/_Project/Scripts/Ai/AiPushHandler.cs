using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class AiPushHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float _pushSpeed = 3f;

        private Ai _ai;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;
        private PushableBox _currentPushedBox = null;

        private const float PUSH_DELAY = 1.5f;

        #region PUSH RESET
        private float _pushResetTimer;
        private const float PUSH_RESET_TIME = 10f;
        #endregion

        #region PROPERTIES
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        public PushableBox CurrentPushedBox => _currentPushedBox;
        public float PushDelay => PUSH_DELAY;
        #endregion

        #region STATICS
        public static float PushDuration { get; private set; }
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            _pushResetTimer = 0f;
            PushDuration = _pushSpeed;

            AiEvents.OnSetCurrentPickaxeSpeed += UpdatePushSpeed;
        }

        private void OnDisable()
        {
            if (_ai == null) return;
            AiEvents.OnSetCurrentPickaxeSpeed -= UpdatePushSpeed;
        }

        #region PUBLICS
        public void AssignCurrentTriggerDirection(Enums.BoxTriggerDirection boxTriggerDirection) => _currentBoxTriggerDirection = boxTriggerDirection;
        public void StartPushingProcess()
        {
            _ai.EnteredPushZone();

            if (_currentPushedBox.IsReadyForPushing && Time.time > _pushResetTimer && _ai.IsInPushZone && !_ai.IsDigging && !_ai.IsPushing && _ai.IsGrounded && _ai.StateManager.CurrentStateType != Enums.AiStateType.Fall)
            {
                _ai.StateManager.SwitchState(_ai.StateManager.PushState);
                _pushResetTimer = Time.time + PUSH_RESET_TIME;
            }
        }
        public void StopPushingProcess()
        {
            _currentPushedBox.Layer.ResetLayer();
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _ai.ExitedPushZone();

            if (_ai.IsGrounded)
                _ai.StateManager.SwitchState(_ai.StateManager.IdleState);
            else
                _ai.StateManager.SwitchState(_ai.StateManager.FallState);
        }
        public void SetPushedBox(PushableBox box) => _currentPushedBox = box;
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void UpdatePushSpeed()
        {
            PushDuration = _pushSpeed - (_pushSpeed * _ai.PowerUpHandler.SpeedRate);
            if (PushDuration < 1f)
                PushDuration = 1f;
        }
        #endregion
    }
}
