using UnityEngine;
using System;

namespace DigFight
{
    public class AdCanvas : MonoBehaviour
    {
        #region BUTTON FUNCTIONS
        public void CloseInterstitialAd()
        {
            AdEventHandler.OnInterstitialClose?.Invoke();
        }
        public void CloseRewardedAd()
        {
            AdEventHandler.OnRewardedAdSuccessful?.Invoke();
        }
        #endregion
    }
}
