using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using System.Collections;

namespace ZestGames
{
    public class ExamplePoint : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private GameObject remainingMoneyContainer;
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        private CanvasGroup _canvasGroup;

        private Collider _collider;
        private readonly int _coreRequiredMoney = 100;
        private float _consumedMoney;

        #region PROPERTIES
        public bool PlayerIsInArea { get; set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < RequiredMoney;
        public int RequiredMoney => _coreRequiredMoney;
        #endregion

        #region SEQUENCE
        private Sequence _bounceSequence;
        private Guid _bounceSequenceID;
        #endregion

        // #######################
        private int _value = 20;
        private readonly WaitForSeconds _spawnMoneyDelay = new WaitForSeconds(0.05f);

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = true;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f;
            remainingMoneyContainer.SetActive(true);
            PlayerIsInArea = false;
            UpdateRemainingMoneyText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartSpawningMoney();
            }
        }

        #region COLLECTING MONEY
        private IEnumerator SpawnMoneyCoroutine()
        {
            int currentCount = 0;
            while (currentCount < _value)
            {
                MoneyCanvas.Instance.SpawnCollectMoney(transform);
                currentCount++;

                yield return _spawnMoneyDelay;
            }
        }

        #region PUBLICS
        public void StartSpawningMoney() => StartCoroutine(SpawnMoneyCoroutine());
        #endregion   
        #endregion

        #region SPENDING MONEY
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (RequiredMoney - _consumedMoney).ToString();
        private void ActivateCanvas()
        {
            _canvasGroup.alpha = 1f;
            StartBounceSequence();
        }

        public void ConsumeMoney(float amount)
        {
            if (amount > (RequiredMoney - _consumedMoney))
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnSpend?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnSpend?.Invoke(RequiredMoney - _consumedMoney);
                    _consumedMoney = RequiredMoney;
                }
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnSpend?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnSpend?.Invoke(amount);
                    _consumedMoney += amount;
                }
            }

            UpdateRemainingMoneyText();

            if (_consumedMoney == RequiredMoney)
            {
                _canvasGroup.alpha = 0.5f;
                remainingMoneyContainer.SetActive(false);
                _collider.enabled = false;
                _consumedMoney = 0;
                UpdateRemainingMoneyText();

                PlayerEvents.OnStopSpendingMoney?.Invoke();
                MoneyCanvas.Instance.StopSpendingMoney();
                ActivateCanvas();
                //AudioHandler.PlayAudio(Enums.AudioType.GraveBuilt);
                //_interactableGround.ActivateGrave();
            }
        }

        #region DOTWEEN FUNCTIONS
        private void StartBounceSequence()
        {
            CreateBounceSequence();
            _bounceSequence.Play();
        }
        private void CreateBounceSequence()
        {
            if (_bounceSequence == null)
            {
                _bounceSequence = DOTween.Sequence();
                _bounceSequenceID = Guid.NewGuid();
                _bounceSequence.id = _bounceSequenceID;

                _bounceSequence.Append(transform.DOShakeScale(1f, 0.01f).OnComplete(() => {
                    DeleteBounceSequence();
                }));
            }
        }
        private void DeleteBounceSequence()
        {
            DOTween.Kill(_bounceSequenceID);
            _bounceSequence = null;
        }
        #endregion
        #endregion
    }
}
