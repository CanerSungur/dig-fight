using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class Pickaxe : MonoBehaviour
    {
        private PlayerDigHandler _playerDigHandler;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject defaultPickaxe;
        [SerializeField] private GameObject leftPickaxe;
        [SerializeField] private GameObject rightPickaxe;

        public void Init(PlayerDigHandler playerDigHandler)
        {
            if (_playerDigHandler == null)
                _playerDigHandler = playerDigHandler;

            EnableDefaultPickaxe();

            PlayerEvents.OnStartDigging += SelectRelevantPickaxe;
            PlayerEvents.OnStopDigging += EnableDefaultPickaxe;
        }

        private void OnDisable()
        {
            if (_playerDigHandler == null) return;
            PlayerEvents.OnStartDigging -= SelectRelevantPickaxe;
            PlayerEvents.OnStopDigging -= EnableDefaultPickaxe;
        }

        #region EVENT HANDLER FUNCTIONS
        private void SelectRelevantPickaxe()
        {
            if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Top)
                EnableDefaultPickaxe();
            else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Left)
                EnableLeftPickaxe();
            else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Right)
                EnableRightPickaxe();
        }
        #endregion

        #region HELPERS
        private void EnableDefaultPickaxe()
        {
            defaultPickaxe.SetActive(true);
            leftPickaxe.SetActive(false);
            rightPickaxe.SetActive(false);
        }
        private void EnableLeftPickaxe()
        {
            defaultPickaxe.SetActive(false);
            leftPickaxe.SetActive(true);
            rightPickaxe.SetActive(false);
        }
        private void EnableRightPickaxe()
        {
            defaultPickaxe.SetActive(false);
            leftPickaxe.SetActive(false);
            rightPickaxe.SetActive(true);
        }
        #endregion
    }
}
