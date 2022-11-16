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
        private CinemachineTransposer _gameplayCMTransposer;

        [Header("-- SHAKE SETUP --")]
        private CinemachineBasicMultiChannelPerlin _gameplayCMBasicPerlin;
        private bool _shakeStarted = false;
        private float _shakeDuration = 1f;
        private float _shakeTimer;

        public static event Action OnShakeCam;

        private void Awake()
        {
            _gameplayCMTransposer = gameplayCM.GetCinemachineComponent<CinemachineTransposer>();
            _gameplayCMBasicPerlin = gameplayCM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
            _shakeTimer = _shakeDuration;

            gameStartCM.Priority = 2;
            gameplayCM.Priority = 1;
            playerUpgradeCM.Priority = 0;
        }

        private void Start()
        {
            GameEvents.OnGameStart += () => gameplayCM.Priority = 3;
            PlayerUpgradeEvents.OnOpenCanvas += () => playerUpgradeCM.Priority = 4;
            PlayerUpgradeEvents.OnCloseCanvas += () => playerUpgradeCM.Priority = 0;
            OnShakeCam += () => _shakeStarted = true;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= () => gameplayCM.Priority = 3;
            PlayerUpgradeEvents.OnOpenCanvas -= () => playerUpgradeCM.Priority = 4;
            PlayerUpgradeEvents.OnCloseCanvas -= () => playerUpgradeCM.Priority = 0;
            OnShakeCam -= () => _shakeStarted = true;
        }

        private void Update()
        {
            ShakeCamForAWhile();
        }

        private void ShakeCamForAWhile()
        {
            if (_shakeStarted)
            {
                _gameplayCMBasicPerlin.m_AmplitudeGain = 1f;

                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    _shakeStarted = false;
                    _shakeTimer = _shakeDuration;

                    _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
                }
            }
        }
    }
}
