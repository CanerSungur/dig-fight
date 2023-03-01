using System;
using UnityEngine;
using TMPro;
using ZestGames;

namespace DigFight
{
    public class PickaxeRewardCanvas : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private CustomButton _continueButton;
        [SerializeField] private float _popUpCloseTimer = 10f;

        #region COMPONENTS
        private Animator _animator;
        private TextMeshProUGUI _closeTimerText;
        private CustomButton _acceptOfferButton;
        #endregion

        #region ANIMATION ID SETUP
        private readonly int _openTabID = Animator.StringToHash("OpenTab");
        private readonly int _openInfoID = Animator.StringToHash("OpenInfo");
        private readonly int _closeInfoID = Animator.StringToHash("CloseInfo");
        #endregion

        private const float POPUP_CLOSE_TIMER = 100f; // seconds
        private float _closeTimer;
        private bool _startTimer;

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                _closeTimerText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                _acceptOfferButton = transform.GetChild(0).GetChild(2).GetComponent<CustomButton>();
            }

            _startTimer = false;
            _acceptOfferButton.onClick.AddListener(() => _acceptOfferButton.TriggerClick(AcceptOffer));
            _continueButton.onClick.AddListener(() => _continueButton.TriggerClick(ContinueAfterAccept));
            GameEvents.OnGameEnd += HandleGameEnd;
        }

        private void OnDisable() 
        {
            if (_animator == null) return;
            _acceptOfferButton.onClick.RemoveListener(() => _acceptOfferButton.TriggerClick(AcceptOffer));
            _continueButton.onClick.RemoveListener(() => _continueButton.TriggerClick(ContinueAfterAccept));
            GameEvents.OnGameEnd -= HandleGameEnd;
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.O))
                OpenCanvas();

            if (_startTimer && GameManager.GameState == Enums.GameState.Started)
            {
                _closeTimer -= Time.deltaTime;

                if (_closeTimer <= 0f)
                {
                    _startTimer = false;
                    CloseCanvas();
                }
                UpdateCloseTimerText();
            }
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            if (_animator.GetBool(_openTabID))
                CloseCanvas();
        }
        #endregion

        #region PRIVATES
        private void AcceptOffer()
        {
            AdEventHandler.OnRewardedAdActivate?.Invoke(() => {
                Time.timeScale = 0f;
                _animator.SetTrigger(_openInfoID);
            });
        }
        private void ContinueAfterAccept()
        {
            Time.timeScale = 1f;
            _animator.SetTrigger(_closeInfoID);
            _animator.SetBool(_openTabID, false);
        }
        private void OpenCanvas()
        {
            _animator.SetBool(_openTabID, true);
            _closeTimer = _popUpCloseTimer;
            _startTimer = true;
        }
        private void CloseCanvas() => _animator.SetBool(_openTabID, false);
        private void UpdateCloseTimerText()
        {
            if (_closeTimer < 0) return;
            float minutes = Mathf.FloorToInt(_closeTimer / 60);
            float seconds = Mathf.FloorToInt(_closeTimer % 60);

            _closeTimerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
        #endregion
    }
}
