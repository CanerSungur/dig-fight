using UnityEngine;
using System.Collections;

namespace ZestGames
{
    public class SettingsMinimalUi : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private bool _isOpen;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject vibrationOn;
        [SerializeField] private GameObject vibrationOff;
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;

        #region AUTO-CLOSE
        private IEnumerator _closeWithDelayEnum;
        private readonly WaitForSeconds _closeDelay = new WaitForSeconds(5f);
        #endregion

        public void Init(UiManager uiManager)
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            _isOpen = false;

            UpdateSoundSprite();
            UpdateVibrationSprite();
        }

        #region MENU
        public void ToggleMenu()
        {
            if (_isOpen)
                CloseMenu();
            else
                OpenMenu();
        }
        private void CloseMenu()
        {
            _animator.SetBool(_openID, false);
            _isOpen = false;

            if (_closeWithDelayEnum != null)
                StopCloseWithDelay();
        }
        private void OpenMenu()
        {
            _animator.SetBool(_openID, true);
            _isOpen = true;

            StartCloseWithDelay();
        }
        #endregion

        #region VIBRATION
        public void ToggleVibration()
        {
            if (SettingsManager.VibrationOn)
                CloseVibration();
            else
                OpenVibration();
        }
        private void OpenVibration()
        {
            SettingsManager.VibrationOn = true;
            UpdateVibrationSprite();
        }
        private void CloseVibration()
        {
            SettingsManager.VibrationOn = false;
            UpdateVibrationSprite();
        }
        #endregion

        #region SOUND
        public void ToggleSound()
        {
            if (SettingsManager.SoundOn)
                CloseSound();
            else
                OpenSound();
        }
        private void OpenSound()
        {
            SettingsManager.SoundOn = true;
            UpdateSoundSprite();
            AudioEvents.OnEnableJetpackSound?.Invoke();
        }
        private void CloseSound()
        {
            SettingsManager.SoundOn = false;
            UpdateSoundSprite();
            AudioEvents.OnDisableJetpackSound?.Invoke();
        }
        #endregion

        #region UPDATERS
        private void UpdateSoundSprite()
        {
            soundOn.SetActive(SettingsManager.SoundOn);
            soundOff.SetActive(!SettingsManager.SoundOn);
        }
        private void UpdateVibrationSprite()
        {
            vibrationOn.SetActive(SettingsManager.VibrationOn);
            vibrationOff.SetActive(!SettingsManager.VibrationOn);
        }
        #endregion

        #region AUTO-CLOSE SECTION
        private void StartCloseWithDelay()
        {
            _closeWithDelayEnum = CloseWithDelayCO();
            StartCoroutine(_closeWithDelayEnum);
        }
        private void StopCloseWithDelay()
        {
            StopCoroutine(_closeWithDelayEnum);
            _closeWithDelayEnum = null;
        }
        private IEnumerator CloseWithDelayCO()
        {
            yield return _closeDelay;
            CloseMenu();
            _closeWithDelayEnum = null;
        }
        #endregion
    }
}
