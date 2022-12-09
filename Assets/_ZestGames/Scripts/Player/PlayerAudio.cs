using UnityEngine;
using UnityEngine.XR;

namespace ZestGames
{
    public class PlayerAudio : MonoBehaviour
    {
        private Player _player;

        #region JETPACK
        private AudioSource _jetpackAudioSource;
        #endregion

        private readonly float _targetPitch = 5f;
        private readonly float _pitchIncrement = 0.05f;
        private readonly float _cooldown = 2f;

        #region COLLECT MONEY
        private float _currentCollectPitch;
        private bool _collectingMoney;
        private float _collectTimer;
        #endregion

        #region SPEND MONEY
        private float _currentSpendPitch;
        private bool _spendingMoney;
        private float _spendTimer;
        #endregion

        public void Init(Player player)
        {
            if (_jetpackAudioSource == null)
            {
                _player = player;
                _jetpackAudioSource = GetComponent<AudioSource>();
                _jetpackAudioSource.loop = true;
                _jetpackAudioSource.Stop();

                AudioManager.Initalize();
            }

            _currentCollectPitch = _currentSpendPitch = 1f;
            _collectingMoney = _spendingMoney = false;
            _collectTimer = _spendTimer = _cooldown;

            AudioEvents.OnPlayCollectMoney += HandleCollectMoney;
            AudioEvents.OnPlaySpendMoney += HandleSpendMoney;
            AudioEvents.OnPlaySwing += Swing;

            PlayerEvents.OnFly += StartJetpackSound;
            PlayerEvents.OnFall += StopJetpackSound;
            PlayerEvents.OnLand += Land;

            GameEvents.OnGameEnd += (Enums.GameEnd ignoreThis) => StopJetpackSound();
        }

        private void OnDisable()
        {
            AudioEvents.OnPlayCollectMoney -= HandleCollectMoney;
            AudioEvents.OnPlaySpendMoney -= HandleSpendMoney;
            AudioEvents.OnPlaySwing -= Swing;

            PlayerEvents.OnFly -= StartJetpackSound;
            PlayerEvents.OnFall -= StopJetpackSound;
            PlayerEvents.OnLand -= Land;

            GameEvents.OnGameEnd -= (Enums.GameEnd ignoreThis) => StopJetpackSound();
        }

        private void Update()
        {
            CheckCollectMoneyPitch();
            CheckStackOreToCartPitch();

            if (_player.PlayerMovement.IsMoving && _player.IsGrounded && !_player.IsPushing)
                AudioManager.PlayAudio(Enums.AudioType.Testing_PlayerMove, 0.1f);
        }

        #region SPEND MONEY FUNCTIONS
        private void HandleSpendMoney()
        {
            AudioManager.PlayAudio(Enums.AudioType.SpendMoney, 0.5f, _currentSpendPitch);
            _spendTimer = _cooldown;
            _spendingMoney = true;
        }
        private void CheckStackOreToCartPitch()
        {
            if (_spendingMoney)
            {
                _spendTimer -= Time.deltaTime;
                if (_spendTimer < 0f)
                {
                    _spendTimer = _cooldown;
                    _spendingMoney = false;
                }
                _currentSpendPitch = Mathf.Lerp(_currentSpendPitch, _targetPitch, _pitchIncrement * Time.deltaTime);
            }
            else
                _currentSpendPitch = 1f;
        }
        #endregion

        #region COLLECT MONEY FUNCTIONS
        private void HandleCollectMoney()
        {
            if (!AudioManager.IsAudioPlaying())
                AudioManager.PlayAudio(Enums.AudioType.CollectMoney, 0.5f, _currentCollectPitch);

            _collectTimer = _cooldown;
            _collectingMoney = true;
        }
        private void CheckCollectMoneyPitch()
        {
            if (_collectingMoney)
            {
                _collectTimer -= Time.deltaTime;
                if (_collectTimer < 0f)
                {
                    _collectTimer = _cooldown;
                    _collectingMoney = false;
                }
                _currentCollectPitch = Mathf.Lerp(_currentCollectPitch, _targetPitch, _pitchIncrement * Time.deltaTime);
            }
            else
                _currentCollectPitch = 1f;
        }
        #endregion

        #region JETPACK
        private void StartJetpackSound() => _jetpackAudioSource.Play();
        private void StopJetpackSound()
        {
            if (_jetpackAudioSource != null)
                _jetpackAudioSource.Stop();
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
