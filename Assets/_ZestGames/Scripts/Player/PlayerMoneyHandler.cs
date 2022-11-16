using System.Collections;
using UnityEngine;

namespace ZestGames
{
    public class PlayerMoneyHandler : MonoBehaviour
    {
        private Player _player;

        #region SPEND MONEY SECTION
        private float _currentSpendMoneyDelay;
        private readonly float _startingSpendMoneyDelay = 0.25f;
        private IEnumerator _spendMoneyEnum;
        // Spend value will increase by 10 in every 5 spend counts to shorten spending time immensely.
        private float _currentMoneySpendValue;
        private int _moneySpendingCount;
        private readonly int _moneyValueMultiplier = 5;
        #endregion

        public bool CanSpendMoney => DataManager.TotalMoney > 0;

        public void Init(Player player)
        {
            _player = player;

            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;

            PlayerEvents.OnStopSpendingMoney += StopSpending;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            PlayerEvents.OnStopSpendingMoney -= StopSpending;
        }

        private IEnumerator SpendMoneyCoroutine(ExamplePoint examplePoint)
        {
            while (examplePoint.MoneyCanBeSpent)
            {
                examplePoint.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                UpdateMoneyValue();
            }
        }
        private void UpdateMoneyValue()
        {
            _moneySpendingCount++;
            if (_moneySpendingCount != 0 && _moneySpendingCount % 5 == 0)
            {
                _currentMoneySpendValue *= _moneyValueMultiplier;
            }
        }

        #region PUBLICS
        public void StartSpending(ExamplePoint examplePoint)
        {
            _spendMoneyEnum = SpendMoneyCoroutine(examplePoint);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (examplePoint.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(examplePoint);
        }
        public void StopSpending()
        {
            StopCoroutine(_spendMoneyEnum);

            // Stop throwing money
            MoneyCanvas.Instance.StopSpendingMoney();
        }
        #endregion
    }
}
