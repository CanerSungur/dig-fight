using UnityEngine;
using System;
using ZestGames;

namespace DigFight
{
    public class AdEventHandler : MonoBehaviour
    {
        public static Action OnInterstitialClose, OnRewardedAdSuccessful;
        public static Action<Action> OnRewardedAdActivate, OnInterstitialActivateForGameEnd, OnInterstitialActivateForMenuChange;

        private Action _currentAction = null;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject rewardedAdCanvas;
        [SerializeField] private GameObject interstitialAdCanvas;

        #region INTERSTITIAL COOLDOWN
        private const float INTERSTITIAL_COOLDOWN = 80f;
        private float _interstitialDelayedTime;
        #endregion

        public void Init(GameManager gameManager)
        {
            _interstitialDelayedTime = 0f;

            rewardedAdCanvas.SetActive(false);
            interstitialAdCanvas.SetActive(false);

            OnRewardedAdActivate += ActivateRewardAd;
            OnRewardedAdSuccessful += RewardAdSuccessful;
            OnInterstitialActivateForGameEnd += ActivateInterstitial;
            OnInterstitialActivateForMenuChange += ActivateInterstitialForMenuChange;
            OnInterstitialClose += CloseInterstitial;
        }

        private void OnDisable()
        {
            OnRewardedAdActivate -= ActivateRewardAd;
            OnRewardedAdSuccessful -= RewardAdSuccessful;
            OnInterstitialActivateForGameEnd -= ActivateInterstitial;
            OnInterstitialActivateForMenuChange -= ActivateInterstitialForMenuChange;
            OnInterstitialClose -= CloseInterstitial;
        }

        #region EVENT HANDLER FUNCTIONS
        private void ActivateRewardAd(Action action)
        {
            Time.timeScale = 0f;
            rewardedAdCanvas.SetActive(true);
            _currentAction = action;
        }
        private void RewardAdSuccessful()
        {
            Time.timeScale = 1f;
            rewardedAdCanvas.SetActive(false);
            _currentAction?.Invoke();
            _currentAction = null;
        }
        private void ActivateInterstitial(Action action)
        {
            Time.timeScale = 0f;
            interstitialAdCanvas.SetActive(true);
            _currentAction = action;
        }
        private void ActivateInterstitialForMenuChange(Action action)
        {
            if (Time.time < _interstitialDelayedTime) 
            {
                print($"<color=#ff3c00>Interstitial cooldown is not finished yet!</color>");
                _currentAction = action;
                _currentAction?.Invoke();
                _currentAction = null;
                return;
            }

            _interstitialDelayedTime = Time.time + INTERSTITIAL_COOLDOWN;

            Time.timeScale = 0f;
            interstitialAdCanvas.SetActive(true);
            _currentAction = action;
        }
        private void CloseInterstitial()
        {
            Time.timeScale = 1f;
            interstitialAdCanvas.SetActive(false);
            _currentAction?.Invoke();
            _currentAction = null;
        }
        #endregion
    }
}
