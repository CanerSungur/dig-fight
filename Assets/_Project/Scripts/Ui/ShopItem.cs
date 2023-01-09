using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class ShopItem : MonoBehaviour
    {
        [Header("-- GENERAL --")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private RectTransform _purchaseButtonImageRect;

        [Header("-- INFO --")]
        [SerializeField] private TextMeshProUGUI _infoAmount;
        [SerializeField] private TextMeshProUGUI _infoRegular;

        [Header("-- PURCHASE --")]
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private Image _priceTypeImage;

        #region COMPONENTS
        private ShopCanvas _shopCanvas;
        private Item _item;
        private CustomButton _purchaseButton;
        private Image _image;
        #endregion

        #region SEQUENCE
        private Sequence _cantAffordSequence;
        private Guid _cantAffordSequenceID;
        private Color _defaultColor;
        private Color _redColor = Color.red;
        #endregion

        public void Init(ShopCanvas shopCanvas, Item item, int index)
        {
            if (_shopCanvas == null)
            {
                _shopCanvas = shopCanvas;
                _item = item;
                _purchaseButton = transform.GetChild(1).GetChild(1).GetComponent<CustomButton>();
                _image = GetComponent<Image>();
                _defaultColor = _image.color;
            }

            SetImage(_item);
            SetPrice(_item);
            SetInfo(_item);

            UiEvents.OnUpdateCollectableText += CheckCanAfford;
            UiEvents.OnUpdateCoinText += CheckCanAfford;

            _purchaseButton.onClick.AddListener(() => _purchaseButton.TriggerClick(PurchaseButtonPressed));
        }

        private void OnDisable()
        {
            if (_shopCanvas == null) return;

            UiEvents.OnUpdateCollectableText -= CheckCanAfford;
            UiEvents.OnUpdateCoinText -= CheckCanAfford;
            
            _purchaseButton.onClick.RemoveListener(() => _purchaseButton.TriggerClick(PurchaseButtonPressed));
        }

        #region EVENT HANDLER FUNCTIONS
        private void CheckCanAfford(float ignoreThis)
        {
            _item.CanAfford = _item.PriceType == Item.PriceTypeEnum.Money && DataManager.TotalMoney >= _item.Price;
        }
        private void CheckCanAfford(int ignoreThis)
        {
            _item.CanAfford = _item.PriceType == Item.PriceTypeEnum.Coin && DataManager.TotalCoin >= _item.Price;
        }
        private void CheckCanAfford()
        {
            if (_item.PriceType == Item.PriceTypeEnum.Money)
                _item.CanAfford = DataManager.TotalMoney >= _item.Price;
            else if (_item.PriceType == Item.PriceTypeEnum.Coin)
                _item.CanAfford = DataManager.TotalCoin >= _item.Price;
        }
        #endregion

        #region BUTTON FUNCTIONS
        private void PurchaseButtonPressed()
        {
            CheckCanAfford();

            if (_item.CanAfford)
            {
                if (_item.PriceType == Item.PriceTypeEnum.Money)
                    CollectableEvents.OnSpend?.Invoke(_item.Price);
                else if (_item.PriceType == Item.PriceTypeEnum.Coin)
                    CoinEvents.OnSpend?.Invoke(_item.Price);

                ActionIfPurchased();
            }
            else
                TriggerCantAffordSequence();
        }
        private void ActionIfPurchased()
        {
            if (_item.ItemType == Item.ItemTypeEnum.PurchaseCoin)
                CoinEvents.OnCollect?.Invoke(_item.PurchaseAmount);
            else if (_item.ItemType == Item.ItemTypeEnum.PurchasePickaxe)
            {
                if (_item.Name == "Silver Pickaxe")
                    Debug.Log("Purchased Silver Pickaxe!");
                else if (_item.Name == "Golden Pickaxe")
                    Debug.Log("Purchased Golden Pickaxe!");
            }
        }
        #endregion

        #region SETTERS
        private void SetImage(Item item)
        {
            _itemImage.sprite = item.Image;
        }
        private void SetPrice(Item item)
        {
            if (item.PriceType == Item.PriceTypeEnum.Money)
                _priceTypeImage.sprite = _shopCanvas.MoneySprite;
            else if (item.PriceType == Item.PriceTypeEnum.Coin)
                _priceTypeImage.sprite = _shopCanvas.CoinSprite;
            else
                Debug.Log("Unknown price type!", this);

            _price.text = item.Price.ToString();
        }
        private void SetInfo(Item item)
        {
            if (item.ItemType == Item.ItemTypeEnum.PurchaseCoin)
            {
                _infoRegular.transform.parent.gameObject.SetActive(false);
                _infoAmount.text = item.PurchaseAmount.ToString();
            }
            else if (item.ItemType == Item.ItemTypeEnum.PurchasePickaxe)
            {
                _infoAmount.transform.parent.gameObject.SetActive(false);
                _infoRegular.text = item.Name;
            }
            else
                Debug.Log("Unknown item type!", this);
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void TriggerCantAffordSequence()
        {
            DeleteCantAffordSequence();
            CreateCantAffordSequence();
            _cantAffordSequence.Play();
        }
        private void CreateCantAffordSequence()
        {
            if (_cantAffordSequence == null)
            {
                _cantAffordSequence = DOTween.Sequence();
                _cantAffordSequenceID = Guid.NewGuid();
                _cantAffordSequence.id = _cantAffordSequenceID;

                _cantAffordSequence.Append(transform.DOShakePosition(2f, new Vector3(5f, 0f, 0f), 10, 0))
                    .OnComplete(DeleteCantAffordSequence);
            }
        }
        private void DeleteCantAffordSequence()
        {
            DOTween.Kill(_cantAffordSequenceID);
            _cantAffordSequence = null;
        }
        #endregion
    }
}
