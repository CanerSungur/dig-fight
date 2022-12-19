using UnityEngine;
using UnityEngine.XR;

namespace ZestGames
{
    public class AiAudio : MonoBehaviour
    {
        private AudioSource _jetpackAudioSource;

        public void Init(Ai ai)
        {
            if (_jetpackAudioSource == null)
            {
                _jetpackAudioSource = GetComponent<AudioSource>();
                _jetpackAudioSource.loop = true;
                _jetpackAudioSource.Stop();
            }

            AiAudioEvents.OnPlaySwing += Swing;
            AiAudioEvents.OnEnableJetpackSound += EnableJetpackSound;
            AiAudioEvents.OnDisableJetpackSound += DisableJetpackSound;
            AiAudioEvents.OnStopJetpackSound += StopJetpackSound;

            AiEvents.OnFly += StartJetpackSound;
            AiEvents.OnFall += StopJetpackSound;
            AiEvents.OnLand += Land;

            GameEvents.OnGameEnd += (Enums.GameEnd ignoreThis) => StopJetpackSound();
        }

        private void OnDisable()
        {
            if (_jetpackAudioSource == null) return;

            AiAudioEvents.OnPlaySwing -= Swing;
            AiAudioEvents.OnEnableJetpackSound -= EnableJetpackSound;
            AiAudioEvents.OnDisableJetpackSound -= DisableJetpackSound;
            AiAudioEvents.OnStopJetpackSound -= StopJetpackSound;

            AiEvents.OnFly -= StartJetpackSound;
            AiEvents.OnFall -= StopJetpackSound;
            AiEvents.OnLand -= Land;

            GameEvents.OnGameEnd -= (Enums.GameEnd ignoreThis) => StopJetpackSound();
        }

        #region JETPACK
        private void StartJetpackSound() => _jetpackAudioSource.Play();
        private void StopJetpackSound()
        {
            if (_jetpackAudioSource != null)
                _jetpackAudioSource.Stop();
        }
        private void EnableJetpackSound()
        {
            _jetpackAudioSource.volume = 0.5f;
        }
        private void DisableJetpackSound()
        {
            _jetpackAudioSource.volume = 0f;
        }
        #endregion

        private void Land()
        {
            AudioManager.PlayAudio(Enums.AudioType.Land, 0.4f);
        }
        private void Swing()
        {
            AudioManager.PlayAudio(Enums.AudioType.Swing);
        }
    }
}
