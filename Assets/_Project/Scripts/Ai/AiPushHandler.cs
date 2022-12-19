using UnityEngine;
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

        #region PUSHING DELAY
        private float _delayedTime;
        private const float PUSH_DELAY = 0.5f;
        #endregion

        #region PROPERTIES
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        public PushableBox CurrentPushedBox => _currentPushedBox;
        #endregion

        #region STATICS
        public static float PushDuration { get; private set; }
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            PushDuration = _pushSpeed;

            AiEvents.OnSetCurrentPickaxeSpeed += UpdatePushSpeed;
        }

        private void OnDisable()
        {
            if (_ai == null) return;
            AiEvents.OnSetCurrentPickaxeSpeed -= UpdatePushSpeed;
        }

        #region PUBLICS
        public void StartPushingProcess(Enums.BoxTriggerDirection triggerDirection)
        {
            //_player.DigHandler.StopDiggingProcess();

            //EnablePickaxe();
            _currentBoxTriggerDirection = triggerDirection;
            _ai.EnteredPushZone();
            _delayedTime = Time.time + PUSH_DELAY;

            //Debug.Log("Side: " + triggerDirection);
        }
        public void StopPushingProcess()
        {
            //DisablePickaxe();
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _ai.ExitedPushZone();
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
