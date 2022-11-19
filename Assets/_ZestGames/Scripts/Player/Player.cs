using UnityEngine;
using DG.Tweening;
using System;
using DigFight;

namespace ZestGames
{
    public class Player : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private LayerMask walkableLayer;
        [SerializeField] private TimerForAction timerForAction;

        #region COMPONENTS
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody == null ? _rigidbody = GetComponent<Rigidbody>() : _rigidbody;
        #endregion

        #region SCRIPT REFERENCES
        private JoystickInput _inputHandler;
        public JoystickInput InputHandler => _inputHandler == null ? _inputHandler = GetComponent<JoystickInput>() : _inputHandler;
        private IPlayerMovement _playerMovement;
        public IPlayerMovement PlayerMovement => _playerMovement == null ? _playerMovement = GetComponent<IPlayerMovement>() : _playerMovement;
        private PlayerAnimationController _animationController;
        public PlayerAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<PlayerAnimationController>() : _animationController;
        private PlayerCollision _collisionHandler;
        public PlayerCollision CollisionHandler => _collisionHandler == null ? _collisionHandler = GetComponent<PlayerCollision>() : _collisionHandler;
        private PlayerMoneyHandler _moneyHandler;
        public PlayerMoneyHandler MoneyHandler => _moneyHandler == null ? _moneyHandler = GetComponent<PlayerMoneyHandler>() : _moneyHandler;
        private PlayerAudio _audioHandler;
        public PlayerAudio AudioHandler => _audioHandler == null ? _audioHandler = GetComponent<PlayerAudio>() : _audioHandler;
        private PlayerEffectHandler _effectHandler;
        public PlayerEffectHandler EffectHandler => _effectHandler == null ? _effectHandler = GetComponent<PlayerEffectHandler>() : _effectHandler;
        private PlayerRotationHandler _rotationHandler;
        public PlayerRotationHandler RotationHandler => _rotationHandler == null ? _rotationHandler = GetComponent<PlayerRotationHandler>() : _rotationHandler;
        private PlayerDigHandler _digHandler;
        public PlayerDigHandler DigHandler => _digHandler == null ? _digHandler = GetComponent<PlayerDigHandler>() : _digHandler;
        #endregion

        #region PROPERTIES
        public bool IsDead { get; private set; }
        public bool IsUpgrading { get; private set; }
        public bool IsInDigZone { get; private set; }
        public bool IsDigging { get; private set; }
        public bool IsFlying { get; private set; }
        public bool UpwardsIsEmpty => !Physics.Raycast(Collider.bounds.center, Vector3.up, Collider.bounds.extents.y + 0.75f, walkableLayer);
        public bool IsTooHigh => !Physics.Raycast(Collider.bounds.center, Vector3.down, Collider.bounds.extents.y + 2f, walkableLayer);
        public bool IsGrounded => Physics.Raycast(Collider.bounds.center, Vector3.down, Collider.bounds.extents.y + 0.01f, walkableLayer);
        public bool CanFly => UpwardsIsEmpty && InputHandler.IsMovingUp;
        public TimerForAction TimerForAction => timerForAction;
        #endregion

        #region SEQUENCE
        private Sequence _upgradeRotationSequence;
        private Guid _upgradeRotationSequenceID;
        private float _upgradeRotationDuration = 1f;
        #endregion

        private void Start()
        {
            CharacterTracker.SetPlayerTransform(transform);
            IsDead = IsUpgrading = IsInDigZone = IsDigging = IsFlying = false;

            InputHandler.Init(this);
            PlayerMovement.Init(this);
            AnimationController.Init(this);
            CollisionHandler.Init(this);
            MoneyHandler.Init(this);
            AudioHandler.Init(this);
            EffectHandler.Init(this);
            timerForAction.Init();
            RotationHandler.Init(this);
            DigHandler.Init(this);

            PlayerUpgradeEvents.OnOpenCanvas += HandleUpgradeStart;
            PlayerUpgradeEvents.OnCloseCanvas += HandleUpgradeEnd;
            PlayerEvents.OnFly += StartFlying;
            PlayerEvents.OnFall += StopFlying;
        }

        private void OnDisable()
        {
            PlayerUpgradeEvents.OnOpenCanvas -= HandleUpgradeStart;
            PlayerUpgradeEvents.OnCloseCanvas -= HandleUpgradeEnd;
            PlayerEvents.OnFly -= StartFlying;
            PlayerEvents.OnFall -= StopFlying;
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandleUpgradeStart()
        {
            IsUpgrading = true;
            StartUpgradeRotationSequence();
        }
        private void HandleUpgradeEnd()
        {
            IsUpgrading = false;
            DeleteUpgradeRotationSequence();
        }
        private void StartFlying() => IsFlying = true;
        private void StopFlying() => IsFlying = false;
        #endregion

        #region PUBLICS
        public void StartedDigging()
        {
            IsDigging = true;
            PlayerEvents.OnStartDigging?.Invoke();
        }
        public void StoppedDigging()
        {
            IsDigging = false;
            PlayerEvents.OnStopDigging?.Invoke();
        }
        public void EnteredDigZone() => IsInDigZone = true;
        public void ExitedDigZone() => IsInDigZone = false;
        #endregion

        private void StartUpgradeRotationSequence()
        {
            DeleteUpgradeRotationSequence();
            CreateUpgradeRotationSequence();
            _upgradeRotationSequence.Play();
        }

        #region DOTWEEN FUNCTIONS
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
        #endregion
    }
}
