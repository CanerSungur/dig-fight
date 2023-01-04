using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ReviveCanvas : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");

        private CustomButton _reviveButton, _noThanksButton;
        private bool _reviveIsClicked;

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                _reviveButton = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<CustomButton>();
                _noThanksButton = transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<CustomButton>();
                _reviveIsClicked = false;
            }
        }

        private void OnDisable()
        {
            if (_animator == null) return;

            _reviveButton.onClick.RemoveListener(ReviveButtonClicked);
            _noThanksButton.onClick.RemoveListener(NoThanksButtonClicked);
        }

        #region BUTTON FUNCTIONS
        private void ReviveButtonClicked()
        {
            _reviveIsClicked = true;
            CloseCanvas();
            _reviveButton.TriggerClick(() => AdEventHandler.OnRewardedAdActivate?.Invoke(() =>
            {
                PlayerEvents.OnRevive?.Invoke();
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.None);
                _reviveIsClicked = false;
            }));
        }
        private void NoThanksButtonClicked()
        {
            CloseCanvas();
            Debug.Log("No Thanks");
        }
        #endregion

        #region OPEN-CLOSE
        private void CloseCanvas() => _animator.SetBool(_openID, false);
        public void OpenCanvas()
        {
            _reviveButton.onClick.AddListener(ReviveButtonClicked);
            _noThanksButton.onClick.AddListener(NoThanksButtonClicked);

            _animator.SetBool(_openID, true);
        }
        #endregion

        #region ANIMATION EVENT FUNCTIONS
        public void FailLevel()
        {
            if (!_reviveIsClicked)
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Fail);
        }
        #endregion
    }
}
