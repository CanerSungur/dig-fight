using UnityEngine;
using System;
using ZestGames;

namespace DigFight
{
    public class AdEventHandler : MonoBehaviour
    {
        public static Action OnInterstitialClose, OnRewardedAdSuccessful;
        public static Action<Action> OnRewardedAdActivate, OnInterstitialActivate;

        private Action _currentAction = null;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject rewardedAdCanvas;
        [SerializeField] private GameObject interstitialAdCanvas;

        public void Init(GameManager gameManager)
        {
            rewardedAdCanvas.SetActive(false);
            interstitialAdCanvas.SetActive(false);

            OnRewardedAdActivate += ActivateRewardAd;
            OnRewardedAdSuccessful += RewardAdSuccessful;
            OnInterstitialActivate += ActivateInterstitial;
            OnInterstitialClose += CloseInterstitial;
        }

        private void OnDisable()
        {
            OnRewardedAdActivate -= ActivateRewardAd;
            OnRewardedAdSuccessful -= RewardAdSuccessful;
            OnInterstitialActivate -= ActivateInterstitial;
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
