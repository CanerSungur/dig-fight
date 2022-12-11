using UnityEngine;
using Lofelt.NiceVibrations;
using ZestGames;

namespace DigFight
{
    public class HapticManager : MonoBehaviour
    {
        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            if (_gameManager == null)
                _gameManager = gameManager;

            if (!IsHapticSupportedOnThisDevice())
            {
                _gameManager.SettingsManager.DisableVibrations();
                Debug.LogWarning("This device does not support HAPTIC!");
                return;
            }

            HapticEvents.OnPlayHitBox += PlayHitBoxHaptic;
            HapticEvents.OnPlayBreakBox += PlayBreakBoxHaptic;
            HapticEvents.OnPlayHitExplosive += PlayHitExplosiveHaptic;
            HapticEvents.OnPlayPush += PlayPushingHaptic;
        }

        private void OnDisable()
        {
            if (!IsHapticSupportedOnThisDevice()) return;

            HapticEvents.OnPlayHitBox -= PlayHitBoxHaptic;
            HapticEvents.OnPlayBreakBox -= PlayBreakBoxHaptic;
            HapticEvents.OnPlayHitExplosive -= PlayHitExplosiveHaptic;
            HapticEvents.OnPlayPush -= PlayPushingHaptic;
        }

        #region HELPERS
        private bool IsHapticSupportedOnThisDevice() => DeviceCapabilities.isVersionSupported;
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void PlayHitBoxHaptic()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        }
        private void PlayBreakBoxHaptic()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        }
        private void PlayHitExplosiveHaptic()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        }
        private void PlayPushingHaptic()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }

        //Soft = new Preset(PresetType.SoftImpact, new float[] { 0.000f, 0.160f },// time
        //                                             new float[] { 0.156f, 0.156f }); // amplitude
        #endregion
    }
}
