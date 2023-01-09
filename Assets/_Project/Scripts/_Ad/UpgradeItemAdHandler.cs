using UnityEngine;
using ZestGames;
using System;

namespace DigFight
{
    public class UpgradeItemAdHandler : MonoBehaviour
    {
        #region PRIVATES
        private UpgradeCanvasItem _upgradeItem;
        private bool _upgradeIsEnabled = true;
        #endregion

        #region PROPERTIES
        public bool UpgradeIsEnabled => _upgradeIsEnabled;
        #endregion

        public void Init(UpgradeCanvasItem upgradeItem)
        {
            if (_upgradeItem == null)
                _upgradeItem = upgradeItem;
        }

        #region PUBLICS
        public void CheckForAd(int level)
        {
            _upgradeIsEnabled = !(level != 1 && level != 2 && IsFibonacci(level));
            //Debug.Log(gameObject.name + ": " + _upgradeIsEnabled);
        }
        public void OpenRewardedAd(Action action)
        {
            AdEventHandler.OnRewardedAdActivate?.Invoke(action);
        }
        #endregion

        #region HEPLERS
        private void CheckFibonacci(int currentLevel)
        {
            int a = 1;
            int b = 1;
            int c;

            for (int i = 1; i <= currentLevel; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }
        }
        //private bool IsFib(int T)
        //{
        //    float root5 = Mathf.Sqrt(5);
        //    float phi = (1 + root5) / 2;

        //    int idx = Mathf.Floor(Mathf.Log(T * root5) / Mathf.Log(phi) + 0.5);
        //    int u = Mathf.Floor(Mathf.Pow(phi, idx) / root5 + 0.5);

        //    return (u == T);
        //}
        private bool IsFibonacci(int w)
        {
            float X1 = 5 * Mathf.Pow(w, 2) + 4;
            float X2 = 5 * Mathf.Pow(w, 2) - 4;

            int X1_sqrt = (int)Mathf.Sqrt(X1);
            int X2_sqrt = (int)Mathf.Sqrt(X2);

            return (X1_sqrt * X1_sqrt == X1) || (X2_sqrt * X2_sqrt == X2);
        }
        #endregion
    }
}
