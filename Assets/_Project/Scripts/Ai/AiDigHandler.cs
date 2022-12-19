using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiDigHandler : MonoBehaviour
    {
        private Ai _ai;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;

        [Header("-- SETUP --")]
        [SerializeField] private Pickaxe pickaxe;

        #region DIGGING DELAY
        private float _delayedTime;
        private const float DIG_DELAY = 2f;
        #endregion

        #region PROPERTIES
        public Ai Ai => _ai;
        public Enums.BoxTriggerDirection CurrentBoxTriggerDirection => _currentBoxTriggerDirection;
        public Pickaxe Pickaxe => pickaxe;
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
                _ai = ai;

            pickaxe.Init(this);
        }

        private void Update()
        {
            if (_ai.IsInDigZone && !_ai.IsDigging && !_ai.IsPushing && Time.time >= _delayedTime)
                _ai.StartedDigging();
        }

        #region PUBLICS
        public void AssignCurrentTriggerDirection(Enums.BoxTriggerDirection boxTriggerDirection) => _currentBoxTriggerDirection = boxTriggerDirection;
        public void StartDiggingProcess()
        {
            _ai.EnteredDigZone();
            _delayedTime = Time.time + DIG_DELAY;
        }
        public void StopDiggingProcess()
        {
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _ai.ExitedDigZone();
        }
        #endregion
    }
}
