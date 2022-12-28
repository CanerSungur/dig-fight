using UnityEngine;
using Cinemachine;
using System;
using DG.Tweening;

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

        #region CAMERA FOV SECTION
        private bool _pushBackCameraForAWhile = false;
        private float _pushBackTimer;
        private float _currentFOV, _currentFollowOffsetX;

        private const float DEFAULT_FOV = 65f;
        private const float PUSHED_BACK_FOV = 75f;
        private const float PUSHED_BACK_DURATION = 3f;

        private const float DEFAULT_FOLLOW_OFFSET_X = 0f;
        private const float PUSHED_BACK_FOLLOW_OFFSET_X = 3f;
        #endregion

        #region CAMERA SHAKE SECTION
        private const float BOX_HIT_AMPLITUDE = 0.5f;
        private const float BOX_BREAK_AMPLITUDE = 0.75f;
        private const float EXPLOSIVE_HIT_AMPLITUDE = 2f;
        #endregion

        #region EVENTS
        public static Action OnBoxHitShake, OnBoxBreakShake, OnExplosiveHitShake, OnPushBackCameraForAWhile, OnPushBackCamera, OnPushInCamera;
        #endregion

        #region SEQUENCE
        private Sequence _pushBackSequence, _pushInSequence;
        private Guid _pushBackSequenceID, _pushInSequenceID;
        private const float PUSHING_DURATION = 6f;
        #endregion

        private void Awake()
        {
            _gameplayCMTransposer = gameplayCM.GetCinemachineComponent<CinemachineTransposer>();
            _gameplayCMBasicPerlin = gameplayCM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
            _shakeTimer = _shakeDuration;
            _currentFollowOffsetX = DEFAULT_FOLLOW_OFFSET_X;
            _currentFOV = DEFAULT_FOV;
            gameplayCM.m_Lens.FieldOfView = _currentFOV;
            _gameplayCMTransposer.m_FollowOffset.x = _currentFollowOffsetX;

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
            OnPushBackCameraForAWhile += PushBackCameraForAWhile;
            OnPushBackCamera += StartPushBackSequence;
            OnPushInCamera += StartPushInSequence;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= () => gameplayCM.Priority = 3;
            PlayerUpgradeEvents.OnOpenCanvas -= () => playerUpgradeCM.Priority = 4;
            PlayerUpgradeEvents.OnCloseCanvas -= () => playerUpgradeCM.Priority = 0;
            OnBoxHitShake -= BoxHitShake;
            OnBoxBreakShake -= BoxBreakShake;
            OnExplosiveHitShake -= ExplosiveHitShake;
            OnPushBackCameraForAWhile -= PushBackCameraForAWhile;
            OnPushBackCamera -= StartPushBackSequence;
            OnPushInCamera -= StartPushInSequence;
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

            if (_pushBackCameraForAWhile)
            {
                _pushBackTimer -= Time.deltaTime;
                if (_pushBackTimer < 0)
                {
                    _pushBackCameraForAWhile = false;
                    _pushBackTimer = PUSHED_BACK_DURATION;

                    StartPushInSequence();
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
        private void PushBackCameraForAWhile()
        {
            StartPushBackSequence();
            _pushBackTimer = PUSHED_BACK_DURATION;
            _pushBackCameraForAWhile = true;
        }
        private void StartPushBackSequence()
        {
            DeletePushInSequence();
            CreatePushBackSequence();
            _pushBackSequence.Play();
        }
        private void StartPushInSequence()
        {
            DeletePushBackSequence();
            CreatePushInSequence();
            _pushInSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreatePushBackSequence()
        {
            if (_pushBackSequence == null)
            {
                _pushBackSequence = DOTween.Sequence();
                _pushBackSequenceID = Guid.NewGuid();
                _pushBackSequence.id = _pushBackSequenceID;

                _pushBackSequence.Append(DOVirtual.Float(_currentFOV, PUSHED_BACK_FOV, PUSHING_DURATION, r => {
                    _currentFOV = r;
                    gameplayCM.m_Lens.FieldOfView = _currentFOV;
                }))
                    .Join(DOVirtual.Float(_currentFollowOffsetX, PUSHED_BACK_FOLLOW_OFFSET_X, PUSHING_DURATION, r => {
                        _currentFollowOffsetX = r;
                        _gameplayCMTransposer.m_FollowOffset.x = _currentFollowOffsetX;
                    }))
                    .OnComplete(() => {
                        _gameplayCMTransposer.m_FollowOffset.x = PUSHED_BACK_FOLLOW_OFFSET_X;
                        gameplayCM.m_Lens.FieldOfView = PUSHED_BACK_FOV;
                        DeletePushBackSequence();
                    });
            }
        }
        private void DeletePushBackSequence()
        {
            DOTween.Kill(_pushBackSequenceID);
            _pushBackSequence = null;
        }
        // #################
        private void CreatePushInSequence()
        {
            if (_pushInSequence == null)
            {
                _pushInSequence = DOTween.Sequence();
                _pushInSequenceID = Guid.NewGuid();
                _pushInSequence.id = _pushInSequenceID;

                _pushInSequence.Append(DOVirtual.Float(_currentFOV, DEFAULT_FOV, PUSHING_DURATION * 0.5f, r => {
                    _currentFOV = r;
                    gameplayCM.m_Lens.FieldOfView = _currentFOV;
                }))
                    .Join(DOVirtual.Float(_currentFollowOffsetX, DEFAULT_FOLLOW_OFFSET_X, PUSHING_DURATION * 0.5f, r => {
                        _currentFollowOffsetX = r;
                        _gameplayCMTransposer.m_FollowOffset.x = _currentFollowOffsetX;
                    }))
                    .OnComplete(() => {
                        _gameplayCMTransposer.m_FollowOffset.x = DEFAULT_FOLLOW_OFFSET_X;
                        gameplayCM.m_Lens.FieldOfView = DEFAULT_FOV;
                        DeletePushBackSequence();
                    });
            }
        }
        private void DeletePushInSequence()
        {
            DOTween.Kill(_pushInSequenceID);
            _pushInSequence = null;
        }
        #endregion
    }
}
