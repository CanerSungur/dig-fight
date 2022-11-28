using System.Collections;
using UnityEngine;

namespace ZestGames
{
    public class MoneyCanvas : MonoBehaviour
    {
        public RectTransform MiddlePointRectTransform { get; private set; }
        private WaitForSeconds _waitforSpendMoneyDelay = new WaitForSeconds(0.1f);
        private IEnumerator _spendMoneyEnum;
        public bool SpendMoneyEnumIsPlaying { get; private set; }

        #region SINGLETON
        public static MoneyCanvas Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            MiddlePointRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            SpendMoneyEnumIsPlaying = false;
        }
        #endregion

        #region COLLECT MONEY
        public void SpawnCollectMoney(Vector3 spawnPosition)
        {
            CollectMoney2D money = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.MoneyCollect2D, Vector3.zero, Quaternion.identity, transform).GetComponent<CollectMoney2D>();
            money.Init(this, spawnPosition);
        }
        #endregion

        #region SPEND MONEY

        #region PRIVATES
        private void SpawnSpendMoney(Transform targetTransform)
        {
            SpendMoney2D money = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.MoneySpend2D, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney2D>();
            money.Init(this, targetTransform);
        }
        #endregion

        #region COROUTINES
        private IEnumerator SpendMoneyCoroutine(ExamplePoint examplePoint)
        {
            while (examplePoint.MoneyCanBeSpent && examplePoint.gameObject.activeSelf)
            {
                SpawnSpendMoney(examplePoint.transform);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
        #endregion

        #region PUBLICS
        public void StartSpendingMoney(ExamplePoint examplePoint)
        {
            if (examplePoint.MoneyCanBeSpent)
            {
                SpendMoneyEnumIsPlaying = true;
                _spendMoneyEnum = SpendMoneyCoroutine(examplePoint);
                StartCoroutine(_spendMoneyEnum);
            }
        }
        public void StopSpendingMoney()
        {
            StopAllCoroutines();
            SpendMoneyEnumIsPlaying = false;
        }
        #endregion
        
        #endregion
    }
}
