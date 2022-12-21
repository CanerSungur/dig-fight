using UnityEngine;
using System;
using ZestCore.Utility;
using DigFight;
using DG.Tweening;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace ZestGames
{
    public class Ai : MonoBehaviour
    {
        #region COMPONENTS
        private Collider coll;
        public Collider Collider => coll == null ? coll = GetComponent<Collider>() : coll;
        private Rigidbody rb;
        public Rigidbody Rigidbody => rb == null ? rb = GetComponent<Rigidbody>() : rb;
        #endregion

        #region SCRIPT REFERENCES
        private AiStateManager _stateManager;
        public AiStateManager StateManager => _stateManager == null ? _stateManager = GetComponent<AiStateManager>() : _stateManager;
        private AiAnimationController animationController;
        public AiAnimationController AnimationController => animationController == null ? animationController = GetComponent<AiAnimationController>() : animationController;
        private AiCollision _collisionHandler;
        public AiCollision CollisionHandler => _collisionHandler == null ? _collisionHandler = GetComponent<AiCollision>() : _collisionHandler;
        private AiAudio _audioHandler;
        public AiAudio AudioHandler => _audioHandler == null ? _audioHandler = GetComponent<AiAudio>() : _audioHandler;
        private AiDigHandler _digHandler;
        public AiDigHandler DigHandler => _digHandler == null ? _digHandler = GetComponent<AiDigHandler>() : _digHandler;
        private AiPushHandler _pushHandler;
        public AiPushHandler PushHandler => _pushHandler == null ? _pushHandler = GetComponent<AiPushHandler>() : _pushHandler;
        private AiPowerUpHandler _powerUpHandler;
        public AiPowerUpHandler PowerUpHandler => _powerUpHandler == null ? _powerUpHandler = GetComponent<AiPowerUpHandler>() : _powerUpHandler;
        private ProgressHandler _progressHandler;
        public ProgressHandler ProgressHandler => _progressHandler == null ? _progressHandler = GetComponent<ProgressHandler>() : _progressHandler;
        private AiSurroundingChecker _surroundingChecker;
        public AiSurroundingChecker SurroundingChecker => _surroundingChecker == null ? _surroundingChecker = GetComponent<AiSurroundingChecker>() : _surroundingChecker;
        #endregion

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float _movementSpeed = 3f;
        [SerializeField] private float _flySpeed = 5f;
        private float _currentMovementSpeed, _currentFlySpeed;

        [Header("-- GROUNDED SETUP --")]
        [SerializeField, Tooltip("Select layers that you want this object to be grounded.")] private LayerMask groundLayerMask;

        #region CONTROLS
        public bool CanMove => GameManager.GameState == Enums.GameState.Started;
        public bool IsGrounded => Physics.Raycast(Collider.bounds.center, Vector3.down, Collider.bounds.extents.y + 0.05f, groundLayerMask);
        public bool IsTooHigh => !Physics.Raycast(Collider.bounds.center, Vector3.down, Collider.bounds.extents.y + 2f, groundLayerMask);
        public bool IsMoving => Rigidbody.velocity.magnitude > 0.05f;
        #endregion

        #region PROPERTIES
        public bool IsDead { get; private set; }
        public bool IsInDigZone { get; private set; }
        public bool IsDigging { get; private set; }
        public bool IsInPushZone { get; private set; }
        public bool IsPushing { get; private set; }
        public bool IsFlying { get; private set; }
        public Transform MeshTransform { get; private set; }
        #endregion

        #region GETTERS
        public float CurrentMovementSpeed => _currentMovementSpeed;
        public float CurrentFlySpeed => _currentFlySpeed;
        #endregion

        #region SEQUENCE
        private Sequence _upgradeRotationSequence, _pushSequence, _kickSequence;
        private Guid _upgradeRotationSequenceID, _pushSequenceID, _kickSequenceID;
        private float _upgradeRotationDuration = 1f;
        //private const float PUSH_SEQUENCE_DURATION = 3f;
        #endregion

        private void Start()
        {
            MeshTransform = transform.GetChild(0);
            IsDead = IsInDigZone = IsDigging = IsFlying = IsInPushZone = IsPushing = false;
            _currentMovementSpeed = _movementSpeed;
            _currentFlySpeed = _flySpeed;

            CharacterTracker.SetAiTransform(transform);

            StateManager.Init(this);
            AnimationController.Init(this);
            CollisionHandler.Init(this);
            AudioHandler.Init(this);
            DigHandler.Init(this);
            PushHandler.Init(this);
            ProgressHandler.Init(this);
            PowerUpHandler.Init(this);

            AiEvents.OnFly += StartFlying;
            AiEvents.OnFall += StopFlying;

            AiEvents.OnSetCurrentPickaxeSpeed += UpdateMotorSpeeds;
        }

        private void OnDisable()
        {
            AiEvents.OnFly -= StartFlying;
            AiEvents.OnFall -= StopFlying;

            AiEvents.OnSetCurrentPickaxeSpeed -= UpdateMotorSpeeds;
        }

        #region EVENT HANDLER FUNCTIONS
        private void StartFlying() => IsFlying = true;
        private void StopFlying() => IsFlying = false;
        private void UpdateMotorSpeeds()
        {
            _currentMovementSpeed = _movementSpeed + (_movementSpeed * PowerUpHandler.SpeedRate);
            _currentFlySpeed = _flySpeed + (_flySpeed * PowerUpHandler.SpeedRate);
        }
        #endregion

        #region PUBLICS
        public void StartedDigging()
        {
            if (GameManager.GameState == Enums.GameState.GameEnded) return;
            //Debug.Log("started dig");
            IsDigging = true;
            AiEvents.OnStartDigging?.Invoke();
            AiAudioEvents.OnStopJetpackSound?.Invoke();
            DigHandler.Pickaxe.OnCannotHit?.Invoke();
        }
        public void StoppedDigging()
        {
            //Debug.Log("stopped dig");
            IsDigging = false;
            AiEvents.OnStopDigging?.Invoke();
            DigHandler.Pickaxe.OnCannotHit?.Invoke();
        }
        public void StartedPushing()
        {
            if (GameManager.GameState == Enums.GameState.GameEnded) return;
            IsPushing = true;

            PushHandler.CurrentPushedBox.CheckSurroundings();
            AnimationController.SelectPushOrKick();

            AiEvents.OnStartPushing?.Invoke();
            DigHandler.Pickaxe.OnCannotHit?.Invoke();
            AiAudioEvents.OnStopJetpackSound?.Invoke();
        }
        public void StoppedPushing()
        {
            IsPushing = false;
            AiEvents.OnStopPushing?.Invoke();
            DigHandler.Pickaxe.OnCannotHit?.Invoke();
        }
        public void EnteredDigZone() => IsInDigZone = true;
        public void ExitedDigZone() => IsInDigZone = false;
        public void EnteredPushZone() => IsInPushZone = true;
        public void ExitedPushZone() => IsInPushZone = false;

        public void StartPushSequence(Enums.BoxTriggerDirection pushDirection)
        {
            CreatePushSequence(pushDirection);
            _pushSequence.Play();
        }
        public void StartKickSequence(Enums.BoxTriggerDirection pushDirection)
        {
            CreateKickSequence(pushDirection);
            _kickSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartUpgradeRotationSequence()
        {
            DeleteUpgradeRotationSequence();
            CreateUpgradeRotationSequence();
            _upgradeRotationSequence.Play();
        }
        private void CreateUpgradeRotationSequence()
        {
            if (_upgradeRotationSequence == null)
            {
                _upgradeRotationSequence = DOTween.Sequence();
                _upgradeRotationSequenceID = Guid.NewGuid();
                _upgradeRotationSequence.id = _upgradeRotationSequenceID;

                _upgradeRotationSequence.Append(transform.DORotate(new Vector3(0f, 180f, 0f), _upgradeRotationDuration)).OnComplete(() =>
                {
                    DeleteUpgradeRotationSequence();
                });
            }
        }
        private void DeleteUpgradeRotationSequence()
        {
            DOTween.Kill(_upgradeRotationSequenceID);
            _upgradeRotationSequence = null;
        }
        // ######################
        private void CreatePushSequence(Enums.BoxTriggerDirection pushDirection)
        {
            if (_pushSequence == null)
            {
                _pushSequence = DOTween.Sequence();
                _pushSequenceID = Guid.NewGuid();
                _pushSequence.id = _pushSequenceID;

                float targetPosX = pushDirection == Enums.BoxTriggerDirection.Left ? transform.position.x - 2f : transform.position.x + 2f;
                _pushSequence.Append(transform.DOMoveX(targetPosX, AiPushHandler.PushDuration))
                    .OnComplete(DeletePushSequence);
            }
        }
        private void DeletePushSequence()
        {
            DOTween.Kill(_pushSequenceID);
            _pushSequence = null;
        }
        // #######################
        private void CreateKickSequence(Enums.BoxTriggerDirection pushDirection)
        {
            if (_kickSequence == null)
            {
                _kickSequence = DOTween.Sequence();
                _kickSequenceID = Guid.NewGuid();
                _kickSequence.id = _kickSequenceID;

                float targetPosX = pushDirection == Enums.BoxTriggerDirection.Left ? transform.position.x - 2f : transform.position.x + 2f;
                _kickSequence.Append(transform.DOMoveX(targetPosX, AiPushHandler.PushDuration * 0.5f))
                    .OnComplete(DeleteKickSequence);
            }
        }
        private void DeleteKickSequence()
        {
            DOTween.Kill(_kickSequenceID);
            _kickSequence = null;
        }
        #endregion
    }
}
