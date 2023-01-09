using UnityEngine;
using ZestGames;
using TMPro;
using ZestCore.Utility;
using System;

namespace DigFight
{
    public class ShopCanvas : MonoBehaviour
    {
        private Animator _animator;

        #region ANIMATION ID SETUP
        private readonly int _openTabID = Animator.StringToHash("OpenTab");
        private readonly int _openShopID = Animator.StringToHash("OpenShop");
        private readonly int _closeShopID = Animator.StringToHash("CloseShop");
        #endregion

        #region BUTTONS
        private CustomButton _openShopButton, _closeShopButton;
        #endregion

        [Header("-- COLLECTABLES --")]
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private RectTransform _coinImageRect;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private RectTransform _moneyImageRect;
        private Transform _coinBgTransform, _moneyBgTransform;

        [Header("-- SHOP ITEM SETUP --")]
        [SerializeField] private ItemDB _shopItemDB;
        [SerializeField] private Transform _contentTransform;

        [Header("-- PREFABS --")]
        [SerializeField] private GameObject _shopItemPrefab;
        [SerializeField] private GameObject _seperatorPrefab;
        [SerializeField] private GameObject _headerPrefab;
        [SerializeField] private CollectableInfoPopup _collectableInfoPopupPrefab;

        [Header("-- SPRITES --")]
        [SerializeField] private Sprite _coinSprite;
        [SerializeField] private Sprite _moneySprite;

        #region PROPERTIES
        public Sprite CoinSprite => _coinSprite;
        public Sprite MoneySprite => _moneySprite;
        public RectTransform MoneyImageRect => _moneyImageRect;
        public RectTransform CoinImageRect => _coinImageRect;
        #endregion

        #region DATA
        private const float SHOP_ITEM_HEIGHT = 250f;
        private const float HEADER_HEIGHT = 69f;
        private const float SEPERATOR_HEIGHT = 128f;
        private int _headerCount, _seperatorCount = 0;
        #endregion

        #region STATIC EVENTS
        public static Action<int> OnCollectCoin, OnSpendCoin;
        public static Action<float> OnCollectMoney, OnSpendMoney;
        #endregion

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                _openShopButton = transform.GetChild(0).GetChild(1).GetComponent<CustomButton>();
                _closeShopButton = transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<CustomButton>();
                _coinBgTransform = _coinImageRect.parent;
                _moneyBgTransform = _moneyImageRect.parent;

                GenerateShopItems();
            }

            OpenShopTab();

            GameEvents.OnGameStart += CloseShopTab;

            OnCollectMoney += TriggerCollectMoneyEffect;
            OnSpendMoney += TriggerSpendMoneyEffect;
            OnCollectCoin += TriggerCollectCoinEffect;
            OnSpendCoin += TriggerSpendCoinEffect;

            _openShopButton.onClick.AddListener(() => _openShopButton.TriggerClick(OpenShop));
            _closeShopButton.onClick.AddListener(() => _closeShopButton.TriggerClick(CloseShop));
        }

        private void OnDisable()
        {
            if (_animator == null) return;

            GameEvents.OnGameStart -= CloseShopTab;

            OnCollectMoney -= TriggerCollectMoneyEffect;
            OnSpendMoney -= TriggerSpendMoneyEffect;
            OnCollectCoin -= TriggerCollectCoinEffect;
            OnSpendCoin -= TriggerSpendCoinEffect;

            _openShopButton.onClick.RemoveListener(() => _openShopButton.TriggerClick(OpenShop));
            _closeShopButton.onClick.RemoveListener(() => _closeShopButton.TriggerClick(CloseShop));
        }

        #region OPEN-CLOSE FUNCTIONS
        private void OpenShopTab() => _animator.SetBool(_openTabID, true);
        private void CloseShopTab() => _animator.SetBool(_openTabID, false);
        private void OpenShop()
        {
            _animator.SetTrigger(_openShopID);
            UpdateCoinText();
            UpdateMoneyText();
        }
        private void CloseShop() => _animator.SetTrigger(_closeShopID);
        #endregion

        #region SHOP ITEM FUNCTIONS
        private void GenerateShopItems()
        {
            Item.ItemTypeEnum lastType = Item.ItemTypeEnum.NotAssigned;
            for (int i = 0; i < _shopItemDB.ShopItemCount; i++)
            {
                Item item = _shopItemDB.GetShopItem(i);

                #region FIRST ROW
                if (i == 0)
                {
                    ShopItemHeader header = Instantiate(_headerPrefab, _contentTransform).GetComponent<ShopItemHeader>();
                    header.SetHeader(item.Header);
                    _headerCount++;
                    lastType = item.ItemType;
                }
                #endregion

                #region ITEM TYPE CHANGE CHECK
                if (lastType != item.ItemType)
                {
                    Instantiate(_seperatorPrefab, _contentTransform);
                    ShopItemHeader header = Instantiate(_headerPrefab, _contentTransform).GetComponent<ShopItemHeader>();
                    header.SetHeader(item.Header);
                    _headerCount++;
                    _seperatorCount++;
                    lastType = item.ItemType;
                }
                #endregion

                ShopItem shopItem = Instantiate(_shopItemPrefab, _contentTransform).GetComponent<ShopItem>();
                shopItem.Init(this, item, i);
            }

            ResizeShopItemContainer();
        }
        private void ResizeShopItemContainer() => _contentTransform.GetComponent<RectTransform>().sizeDelta =
            Vector2.up * ((SHOP_ITEM_HEIGHT * _shopItemDB.ShopItemCount) +
            (_seperatorCount * SEPERATOR_HEIGHT) +
            (_headerCount * HEADER_HEIGHT));
        #endregion

        #region MONEY FUNCTIONS
        private void TriggerCollectMoneyEffect(float amount)
        {
            CollectableInfoPopup infoPopup = Instantiate(_collectableInfoPopupPrefab, _moneyBgTransform);
            infoPopup.TriggerCollectSequence(_moneySprite, (int)amount, () => {
                UpdateMoneyText();
                DOTweenUtils.ChangeTextColorForAWhile(_moneyText, Color.white, Color.green);
            });
        }
        private void TriggerSpendMoneyEffect(float amount)
        {
            DOTweenUtils.ChangeTextColorForAWhile(_moneyText, Color.white, Color.red);
            UpdateMoneyText();

            CollectableInfoPopup infoPopup = Instantiate(_collectableInfoPopupPrefab, _moneyBgTransform);
            infoPopup.TriggerSpendSequence(_moneySprite, (int)amount);
        }
        private void UpdateMoneyText()
        {
            _moneyText.text = DataManager.TotalMoney.ToString();
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
        #endregion

        #region COIN FUNCTIONS
        private void TriggerCollectCoinEffect(int amount)
        {
            CollectableInfoPopup infoPopup = Instantiate(_collectableInfoPopupPrefab, _coinBgTransform);
            infoPopup.TriggerCollectSequence(_coinSprite, amount, () => {
                UpdateCoinText();
                DOTweenUtils.ChangeTextColorForAWhile(_coinText, Color.white, Color.green);
            });
        }
        private void TriggerSpendCoinEffect(int amount)
        {
            DOTweenUtils.ChangeTextColorForAWhile(_coinText, Color.white, Color.red);
            UpdateCoinText();

            CollectableInfoPopup infoPopup = Instantiate(_collectableInfoPopupPrefab, _coinBgTransform);
            infoPopup.TriggerSpendSequence(_coinSprite, amount);
        }
        private void UpdateCoinText()
        {
            _coinText.text = DataManager.TotalCoin.ToString();
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
        #endregion
    }
}
