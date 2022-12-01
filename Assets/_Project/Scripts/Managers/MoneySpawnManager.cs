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
        }

        private void OnDisable()
        {
            CollectableEvents.OnSpawnMoney -= SpawnMoney;
        }

        private void SpawnMoney(int amount, Vector3 boxPosition)
        {
            for (int i = 0; i < amount; i++)
                MoneyCanvas.Instance.SpawnCollectMoney(boxPosition + _textOffset);

            SpawnMoneyText(amount, boxPosition);
        }
        private void SpawnMoneyText(int amount, Vector3 boxPosition)
        {
            TextMeshPro moneyText = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.MoneyTextDisplay, boxPosition + _textOffset, Quaternion.identity).GetComponentInChildren<TextMeshPro>();
            Animation moneyTextAnimation = moneyText.GetComponentInParent<Animation>();
            moneyTextAnimation.Rewind();
            moneyTextAnimation.Play();
            moneyText.text = "+" + amount * DataManager.MoneyValue;
        }
    }
}
