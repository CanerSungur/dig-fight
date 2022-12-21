using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class AiDigHandler : MonoBehaviour
    {
        private Ai _ai;
        private Enums.BoxTriggerDirection _currentBoxTriggerDirection;

        #region DIG CHANCE
        private int _currentDiggedBoxCount = 0;
        private bool _decidedToDig = false;
        private const int MAX_DIG_COUNT_FOR_RESET = 5;
        private const int DIG_CHANCE = 0;
        private const int DIG_CHANCE_REDUCTION = 0;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private Pickaxe pickaxe;

        //#region DIGGING DELAY
        //private float _delayedTime;
        //private const float DIG_DELAY = 2f;
        //#endregion

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

        //private void Update()
        //{
        //    if (_ai.IsInDigZone && !_ai.IsDigging && !_ai.IsPushing && _decidedToDig)
        //    {
        //        _ai.StateManager.SwitchState(_ai.StateManager.DigState);
        //    }
        //}

        #region PUBLICS
        public void AssignCurrentTriggerDirection(Enums.BoxTriggerDirection boxTriggerDirection) => _currentBoxTriggerDirection = boxTriggerDirection;
        public void StartDiggingProcess()
        {
            _ai.EnteredDigZone();
            //_delayedTime = Time.time + DIG_DELAY;
            int chance = DIG_CHANCE - (_currentDiggedBoxCount * DIG_CHANCE_REDUCTION);
            chance = chance < 0 ? 0 : chance;
            _decidedToDig = RNG.RollDice(chance);

            if (_decidedToDig && _ai.IsInDigZone && !_ai.IsDigging && !_ai.IsPushing && _ai.IsGrounded && _ai.StateManager.CurrentStateType != Enums.AiStateType.Fall)
            {
                _ai.StateManager.SwitchState(_ai.StateManager.DigState);
                _currentDiggedBoxCount++;

                if (_currentDiggedBoxCount >= MAX_DIG_COUNT_FOR_RESET)
                    _currentDiggedBoxCount = 0;

                _decidedToDig = false;
            }
        }
        public void StopDiggingProcess()
        {
            _currentBoxTriggerDirection = Enums.BoxTriggerDirection.None;
            _ai.ExitedDigZone();
        }
        #endregion
    }
}
