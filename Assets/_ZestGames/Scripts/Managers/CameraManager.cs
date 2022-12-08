using UnityEngine;
using Cinemachine;
using System;

namespace ZestGames
{
    public class CameraManager : MonoBehaviour
    {
        [Header("-- CAMERA SETUP --")]
        [SerializeField] private CinemachineVirtualCamera gameStartCM;
        [SerializeField] private CinemachineVirtualCamera gameplayCM;
        [SerializeField] private CinemachineVirtualCamera playerUpgradeCM;
        [SerializeField] private CinemachineVirtualCamera boostCM;
        private CinemachineTransposer _gameplayCMTransposer;

        [Header("-- SHAKE SETUP --")]
        private CinemachineBasicMultiChannelPerlin _gameplayCMBasicPerlin;
        private bool _shakeStarted = false;
        private float _shakeDuration = 0.5f;
        private float _shakeTimer;

        private bool _pushBackCamera = false;
        private float _pushBackDuration = 2f;
        private float _pushBackTimer;

        private bool _zoomIn = false;
        private float _zoomInDuration = 2f;
        private float _zoomInTimer;

        private const float BOX_HIT_AMPLITUDE = 0.5f;
        private const float BOX_BREAK_AMPLITUDE = 0.75f;
        private const float EXPLOSIVE_HIT_AMPLITUDE = 2f;

        public static Action OnBoxHitShake, OnBoxBreakShake, OnExplosiveHitShake, OnBoxPushed, OnBoostPickedUp;

        private void Awake()
        {
            _gameplayCMTransposer = gameplayCM.GetCinemachineComponent<CinemachineTransposer>();
            _gameplayCMBasicPerlin = gameplayCM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
            _shakeTimer = _shakeDuration;
            _pushBackTimer = _pushBackDuration;
            _zoomInTimer = _zoomInDuration;

            gameStartCM.Priority = 2;
            gameplayCM.Priority = 1;
            playerUpgradeCM.Priority = 0;
            boostCM.Priority = 0;
        }

        private void Start()
        {
            GameEvents.OnGameStart += () => gameplayCM.Priority = 3;
            PlayerUpgradeEvents.OnOpenCanvas += () => playerUpgradeCM.Priority = 4;
            PlayerUpgradeEvents.OnCloseCanvas += () => playerUpgradeCM.Priority = 0;
            OnBoxHitShake += BoxHitShake;
            OnBoxBreakShake += BoxBreakShake;
            OnExplosiveHitShake += ExplosiveHitShake;
            OnBoxPushed += PushBackCamera;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= () => gameplayCM.Priority = 3;
            PlayerUpgradeEvents.OnOpenCanvas -= () => playerUpgradeCM.Priority = 4;
            PlayerUpgradeEvents.OnCloseCanvas -= () => playerUpgradeCM.Priority = 0;
            OnBoxHitShake -= BoxHitShake;
            OnBoxBreakShake -= BoxBreakShake;
            OnExplosiveHitShake -= ExplosiveHitShake;
            OnBoxPushed -= PushBackCamera;
        }

        private void Update()
        {
            ShakeCamForAWhile();
        }

        private void ShakeCamForAWhile()
        {
            if (_shakeStarted)
            {
                //_gameplayCMBasicPerlin.m_AmplitudeGain = 1f;

                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    _shakeStarted = false;
                    _shakeTimer = _shakeDuration;

                    _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
                }
            }

            if (_pushBackCamera)
            {
                _pushBackTimer -= Time.deltaTime;
                if (_pushBackTimer < 0)
                {
                    _pushBackCamera = false;
                    _pushBackTimer = _pushBackDuration;

                    gameStartCM.Priority = 2;
                }
            }

            if (_zoomIn)
            {
                _zoomInTimer -= Time.deltaTime;
                if (_zoomInTimer < 0)
                {
                    _zoomIn = false;
                    _zoomInTimer = _zoomInDuration;

                    boostCM.Priority = 0;
                }
            }
        }
        #region EVENT HANDLER FUNCTIONS
        private void BoxHitShake()
        {
            _shakeStarted = true;
            _gameplayCMBasicPerlin.m_AmplitudeGain = BOX_HIT_AMPLITUDE;
        }
        private void BoxBreakShake()
        {
            _shakeStarted = true;
            _gameplayCMBasicPerlin.m_AmplitudeGain = BOX_BREAK_AMPLITUDE;
        }
        private void ExplosiveHitShake()
        {
            _shakeStarted = true;
            _gameplayCMBasicPerlin.m_AmplitudeGain = EXPLOSIVE_HIT_AMPLITUDE;
            _shakeTimer = _shakeDuration * 2f;
        }
        private void PushBackCamera()
        {
            gameStartCM.Priority = 5;
            _pushBackCamera = true;
        }
        private void ZoomInForAWhile()
        {
            boostCM.Priority = 6;
            _zoomIn = true;
        }
        #endregion
    }
}
