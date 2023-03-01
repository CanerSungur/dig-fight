using System;
using UnityEngine;
using ZestGames;
using TMPro;

namespace DigFight
{
    public class MoneySpawnManager : MonoBehaviour
    {
        private readonly Vector3 _textOffset = new Vector3(0f, 0f, -2f);

        public void Init(GameManager gameManager)
        {
            CollectableEvents.OnSpawnMoney += SpawnMoney;
            CoinEvents.OnSpawnCoin += SpawnCoin;
        }

        private void OnDisable()
        {
            CollectableEvents.OnSpawnMoney -= SpawnMoney;
            CoinEvents.OnSpawnCoin -= SpawnCoin;
        }

        #region EVENT HANDLER FUNCTIONS
        private void SpawnCoin(int amount, Vector3 boxPosition)
        {
            for (int i = 0; i < amount; i++)
                MoneyCanvas.Instance.SpawnCollectCoin(boxPosition + _textOffset);
        }
        private void SpawnMoney(int amount, Vector3 boxPosition)
        {
            for (int i = 0; i < amount; i++)
                MoneyCanvas.Instance.SpawnCollectMoney(boxPosition + _textOffset);

            SpawnMoneyText(amount, boxPosition);
        }
        #endregion

        #region HELPERS
        private void SpawnMoneyText(int amount, Vector3 boxPosition)
        {
            TextMeshPro moneyText = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.MoneyTextDisplay, boxPosition + _textOffset, Quaternion.identity).GetComponentInChildren<TextMeshPro>();
            Animation moneyTextAnimation = moneyText.GetComponentInParent<Animation>();
            moneyTextAnimation.Rewind();
            moneyTextAnimation.Play();
            moneyText.text = "+" + amount * DataManager.MoneyValue;
        }
        #endregion
    }
}
