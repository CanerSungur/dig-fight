using UnityEngine;
using TMPro;
using ZestGames;

namespace DigFight
{
    public class PowerUpCanvas : MonoBehaviour
    {
        private ChestBase _activatedChest;

        [Header("-- IMAGE SETUP --")]
        [SerializeField] private GameObject durabilityImageObj;
        [SerializeField] private GameObject powerImageObj;
        [SerializeField] private GameObject speedImageObj;

        private TextMeshProUGUI _powerUpNameText;
        private TextMeshProUGUI _powerUpValueText;

        private CustomButton _emptySpaceButton;
        private CustomButton _continueButton;

        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();

                _powerUpNameText = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                _powerUpValueText = transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
                _continueButton = transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<CustomButton>();
                _emptySpaceButton = transform.GetChild(0).GetChild(0).GetComponent<CustomButton>();
            }

            _emptySpaceButton.onClick.AddListener(() => _continueButton.TriggerClick(CloseCanvas));
            _continueButton.onClick.AddListener(() => _continueButton.TriggerClick(CloseCanvas));
            PowerUpEvents.OnPickaxeDurabilityTaken += HandlePowerUpPickUp;
            PowerUpEvents.OnPickaxeSpeedTaken += HandlePowerUpPickUp;
            PowerUpEvents.OnPickaxePowerTaken += HandlePowerUpPickUp;
        }

        private void OnDisable()
        {
            if (_animator == null) return;

            _emptySpaceButton.onClick.RemoveListener(() => _continueButton.TriggerClick(CloseCanvas));
            _continueButton.onClick.RemoveListener(() => _continueButton.TriggerClick(CloseCanvas));
            PowerUpEvents.OnPickaxeDurabilityTaken -= HandlePowerUpPickUp;
            PowerUpEvents.OnPickaxeSpeedTaken -= HandlePowerUpPickUp;
            PowerUpEvents.OnPickaxePowerTaken -= HandlePowerUpPickUp;
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandlePowerUpPickUp(ChestBase activatedChest, PowerUp powerUp)
        {
            OpenCanvas();

            _activatedChest = activatedChest;

            SetPowerUpTexts(powerUp);
            SetPowerUpImage(_activatedChest.ChestType);
        }
        #endregion

        #region HELPERS
        private void SetPowerUpTexts(PowerUp powerUp)
        {
            _powerUpNameText.text = powerUp.Name;
            if (powerUp.Name == "SPEED" || powerUp.Name == "POWER")
                _powerUpValueText.text = "+%" + (powerUp.IncrementValue * 100f);
            else
                _powerUpValueText.text = "+" + powerUp.IncrementValue;
        }
        private void SetPowerUpImage(Enums.ChestType chestType)
        {
            durabilityImageObj.SetActive(chestType == Enums.ChestType.PickaxeDurability);
            powerImageObj.SetActive(chestType == Enums.ChestType.PickaxePower);
            speedImageObj.SetActive(chestType == Enums.ChestType.PickaxeSpeed);
        }
        #endregion

        #region OPEN-CLOSE
        private void CloseCanvas() => _animator.SetBool(_openID, false);
        private void OpenCanvas()
        {
            _animator.SetBool(_openID, true);
            Time.timeScale = 0f;
        }
        #endregion

        #region ANIMATION EVENT FUNCTIONS
        public void ActivatePowerUp()
        {
            Time.timeScale = 1f;

            if (_activatedChest.ChestType == Enums.ChestType.PickaxeDurability)
                PlayerEvents.OnActivatePickaxeDurability?.Invoke(_activatedChest.PowerUp);
            else if (_activatedChest.ChestType == Enums.ChestType.PickaxePower)
                PlayerEvents.OnActivatePickaxePower?.Invoke(_activatedChest.PowerUp);
            else if (_activatedChest.ChestType == Enums.ChestType.PickaxeSpeed)
                PlayerEvents.OnActivatePickaxeSpeed?.Invoke(_activatedChest.PowerUp);

            _activatedChest.StartClosingChestSequence();
        }
        #endregion
    }
}
