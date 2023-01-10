using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ZestGames;
using DG.Tweening;
using System;

namespace DigFight
{
    public class PickaxeUpgradeItemUi : MonoBehaviour
    {
        [Header("-- GENERAL --")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private RectTransform _purchaseButtonImageRect;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("-- INFO --")]
        [SerializeField] private TextMeshProUGUI _infoTitleText;
        [SerializeField] private TextMeshProUGUI _infoDescriptionText;

        [Header("-- PURCHASE --")]
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Image _priceTypeImage;

        #region COMPONENTS
        private PickaxeUpgradeCanvas _pickaxeUpgradeCanvas;
        private PickaxeUpgrade _pickaxeUpgrade;
        private CustomButton _purchaseButton;
        private Image _image;
        #endregion

        #region SEQUENCE
        private Sequence _cantAffordSequence;
        private Guid _cantAffordSequenceID;
        private Color _defaultColor;
        private Color _redColor = Color.red;
        #endregion

        public void Init(PickaxeUpgradeCanvas pickaxeUpgradeCanvas, PickaxeUpgrade pickaxeUpgrade, int index)
        {
            if (_pickaxeUpgradeCanvas == null)
            {
                _pickaxeUpgradeCanvas = pickaxeUpgradeCanvas;
                _pickaxeUpgrade = pickaxeUpgrade;
                _purchaseButton = transform.GetChild(1).GetChild(1).GetComponent<CustomButton>();
                _image = GetComponent<Image>();
                _defaultColor = _image.color;
            }

            _pickaxeUpgrade.LoadLevel();

            SetImage(_pickaxeUpgrade);
            SetPrice(_pickaxeUpgrade);
            SetInfo(_pickaxeUpgrade);
            SetLevel(_pickaxeUpgrade);

            UiEvents.OnUpdateCollectableText += CheckCanAfford;
            UiEvents.OnUpdateCoinText += CheckCanAfford;

            _purchaseButton.onClick.AddListener(() => _purchaseButton.TriggerClick(PurchaseButtonPressed));
        }

        private void OnDisable()
        {
            if (_pickaxeUpgradeCanvas == null) return;

            UiEvents.OnUpdateCollectableText -= CheckCanAfford;
            UiEvents.OnUpdateCoinText -= CheckCanAfford;
            
            _purchaseButton.onClick.RemoveListener(() => _purchaseButton.TriggerClick(PurchaseButtonPressed));
        }

        #region EVENT HANDLER FUNCTIONS
        private void CheckCanAfford(float ignoreThis)
        {
            _pickaxeUpgrade.CanAfford = _pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Money && DataManager.TotalMoney >= _pickaxeUpgrade.Price;
        }
        private void CheckCanAfford(int ignoreThis)
        {
            _pickaxeUpgrade.CanAfford = _pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Coin && DataManager.TotalCoin >= _pickaxeUpgrade.Price;
        }
        private void CheckCanAfford()
        {
            if (_pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Money)
                _pickaxeUpgrade.CanAfford = DataManager.TotalMoney >= _pickaxeUpgrade.Price;
            else if (_pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Coin)
                _pickaxeUpgrade.CanAfford = DataManager.TotalCoin >= _pickaxeUpgrade.Price;
        }
        #endregion

        #region BUTTON FUNCTIONS
        private void PurchaseButtonPressed()
        {
            CheckCanAfford();

            if (_pickaxeUpgrade.CanAfford)
            {
                if (_pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Money)
                    CollectableEvents.OnSpend?.Invoke(_pickaxeUpgrade.Price);
                else if (_pickaxeUpgrade.PriceType == PickaxeUpgrade.PriceTypeEnum.Coin)
                    CoinEvents.OnSpend?.Invoke(_pickaxeUpgrade.Price);

                ActionIfPurchased();
            }
            else
                TriggerCantAffordSequence();
        }
        private void ActionIfPurchased()
        {
            _pickaxeUpgrade.Upgrade();

            SetPrice(_pickaxeUpgrade);
            SetLevel(_pickaxeUpgrade);
        }
        #endregion

        #region SETTERS
        private void SetImage(PickaxeUpgrade item) => _itemImage.sprite = item.Image;
        private void SetPrice(PickaxeUpgrade item)
        {
            if (item.PriceType == PickaxeUpgrade.PriceTypeEnum.Money)
                _priceTypeImage.sprite = _pickaxeUpgradeCanvas.MoneySprite;
            else if (item.PriceType == PickaxeUpgrade.PriceTypeEnum.Coin)
                _priceTypeImage.sprite = _pickaxeUpgradeCanvas.CoinSprite;
            else
                Debug.Log("Unknown price type!", this);

            _priceText.text = item.Price.ToString();
        }
        private void SetInfo(PickaxeUpgrade item)
        {
            _infoTitleText.text = item.Title;
            _infoDescriptionText.text = item.Description;
        }
        private void SetLevel(PickaxeUpgrade item) => _levelText.text = "Level " + item.Level;
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
