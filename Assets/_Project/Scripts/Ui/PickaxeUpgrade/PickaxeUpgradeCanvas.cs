using UnityEngine;
using ZestGames;
using TMPro;
using ZestCore.Utility;
using System;

namespace DigFight
{
    public class PickaxeUpgradeCanvas : MonoBehaviour
    {
        #region COMPONENTS
        private Animator _animator;
        private UiManager _uiManager;
        #endregion

        #region ANIMATION ID SETUP
        private readonly int _openTabID = Animator.StringToHash("OpenTab");
        private readonly int _openPickaxeUpgradeID = Animator.StringToHash("OpenPickaxeUpgrade");
        private readonly int _closePickaxeUpgradeID = Animator.StringToHash("ClosePickaxeUpgrade");
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

        [Header("-- PICKAXE UPGRADE ITEM SETUP --")]
        [SerializeField] private PickaxeBase[] _pickaxes;
        [SerializeField] private Transform _contentTransform;

        [Header("-- PREFABS --")]
        [SerializeField] private GameObject _pickaxeUpgradeItemPrefab;
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
        private const float PICKAXE_UPGRADE_ITEM_HEIGHT = 250f;
        private const float HEADER_HEIGHT = 69f;
        private const float SEPERATOR_HEIGHT = 128f;
        private int _upgradeItemCount, _headerCount, _seperatorCount = 0;
        #endregion

        #region STATIC EVENTS
        public static Action<int> OnSpendCoin;
        public static Action<float> OnSpendMoney;
        #endregion

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
            {
                _uiManager = uiManager;
                _animator = GetComponent<Animator>();
                _openShopButton = transform.GetChild(0).GetChild(1).GetComponent<CustomButton>();
                _closeShopButton = transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<CustomButton>();
                _coinBgTransform = _coinImageRect.parent;
                _moneyBgTransform = _moneyImageRect.parent;

                GeneratePickaxeUpgradeItems();
            }

            OpenPickaxeUpgradeTab();

            GameEvents.OnGameStart += ClosePickaxeUpgradeTab;

            OnSpendMoney += TriggerSpendMoneyEffect;
            OnSpendCoin += TriggerSpendCoinEffect;

            _openShopButton.onClick.AddListener(() => _openShopButton.TriggerClick(OpenPickaxeUpgrade));
            _closeShopButton.onClick.AddListener(() => _closeShopButton.TriggerClick(ClosePickaxeUpgrade));
        }

        private void OnDisable()
        {
            if (_animator == null) return;

            GameEvents.OnGameStart -= ClosePickaxeUpgradeTab;

            OnSpendMoney -= TriggerSpendMoneyEffect;
            OnSpendCoin -= TriggerSpendCoinEffect;

            _openShopButton.onClick.RemoveListener(() => _openShopButton.TriggerClick(OpenPickaxeUpgrade));
            _closeShopButton.onClick.RemoveListener(() => _closeShopButton.TriggerClick(ClosePickaxeUpgrade));
        }

        #region OPEN-CLOSE FUNCTIONS
        public void OpenPickaxeUpgradeTab() => _animator.SetBool(_openTabID, true);
        public void ClosePickaxeUpgradeTab() => _animator.SetBool(_openTabID, false);
        private void OpenPickaxeUpgrade()
        {
            _uiManager.CloseShopTab();
            _animator.SetTrigger(_openPickaxeUpgradeID);
            UpdateCoinText();
            UpdateMoneyText();
        }
        private void ClosePickaxeUpgrade() 
        {
            _uiManager.OpenShopTab();
            _animator.SetTrigger(_closePickaxeUpgradeID);
        } 
        #endregion

        #region PICKAXE UPGRADE ITEM FUNCTIONS
        private void GeneratePickaxeUpgradeItems()
        {
            Enums.PickaxeType lastType = Enums.PickaxeType.NotAssigned;
            
            for (int i = 0; i < _pickaxes.Length; i++)
            {
                for (int j = 0; j < _pickaxes[i].Upgrades.Length; j++)
                {
                    PickaxeUpgrade pickaxeUpgrade = _pickaxes[i].Upgrades[j];
                    _upgradeItemCount++;

                    #region FIRST ROW
                    if (i == 0 && j == 0)
                    {
                        ItemHeader header = Instantiate(_headerPrefab, _contentTransform).GetComponent<ItemHeader>();
                        header.SetHeader(pickaxeUpgrade.Name);
                        _headerCount++;
                        lastType = pickaxeUpgrade.OwnerType;
                    }
                    #endregion

                    #region ITEM TYPE CHANGE CHECK
                    if (lastType != pickaxeUpgrade.OwnerType)
                    {
                        Instantiate(_seperatorPrefab, _contentTransform);
                        ItemHeader header = Instantiate(_headerPrefab, _contentTransform).GetComponent<ItemHeader>();
                        header.SetHeader(pickaxeUpgrade.Name);
                        _headerCount++;
                        _seperatorCount++;
                        lastType = pickaxeUpgrade.OwnerType;
                    }
                    #endregion

                    PickaxeUpgradeItemUi pickaxeUpgradeItemUi = Instantiate(_pickaxeUpgradeItemPrefab, _contentTransform).GetComponent<PickaxeUpgradeItemUi>();
                    pickaxeUpgradeItemUi.Init(this, pickaxeUpgrade, i);
                    }
            }

            ResizeShopItemContainer();
        }
        private void ResizeShopItemContainer() => _contentTransform.GetComponent<RectTransform>().sizeDelta =
            Vector2.up * ((PICKAXE_UPGRADE_ITEM_HEIGHT * _upgradeItemCount) +
            (_seperatorCount * SEPERATOR_HEIGHT) +
            (_headerCount * HEADER_HEIGHT));
        #endregion

        #region MONEY FUNCTIONS
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
