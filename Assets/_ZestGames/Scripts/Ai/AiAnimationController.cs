using DG.Tweening;
using UnityEngine;
using System;
using DigFight;
using Random = UnityEngine.Random;

namespace ZestGames
{
    public class AiAnimationController : MonoBehaviour
    {
        #region COMPONENTS
        private Ai _ai;
        private Animator _animator;
        private AiAnimationEventListener _animationEventListener;
        #endregion

        #region PROPERTIES
        public Ai Ai => _ai;
        public Animator Animator => _animator;
        #endregion

        #region ANIMATION PARAMETERS
        // Booleans
        private readonly int _moveID = Animator.StringToHash("Move");
        private readonly int _groundedID = Animator.StringToHash("Grounded");
        private readonly int _flyingID = Animator.StringToHash("Flying");
        private readonly int _tooHighID = Animator.StringToHash("TooHigh");
        private readonly int _diggingID = Animator.StringToHash("Digging");
        private readonly int _pushingID = Animator.StringToHash("Pushing");
        private readonly int _kickingID = Animator.StringToHash("Kicking");

        // Triggers
        private readonly int _dieID = Animator.StringToHash("Die");
        private readonly int _winID = Animator.StringToHash("Win");
        private readonly int _loseID = Animator.StringToHash("Lose");
        private readonly int _cheerID = Animator.StringToHash("Cheer");
        private readonly int _staggerID = Animator.StringToHash("Stagger");

        // Integers
        private readonly int _cheerIndexID = Animator.StringToHash("CheerIndex");
        private readonly int _loseIndexID = Animator.StringToHash("LoseIndex");
        private readonly int _digSideIndexID = Animator.StringToHash("DigSideIndex");

        // Floats
        private readonly int _digSpeedID = Animator.StringToHash("DigSpeed");
        private readonly int _scaleRateID = Animator.StringToHash("ScaleRate");
        private readonly int _pushSpeedRateID = Animator.StringToHash("PushSpeedRate");
        private readonly int _kickSpeedRateID = Animator.StringToHash("KickSpeedRate");
        #endregion

        #region DIG SIDE DATA
        private const int DIG_TOP_SIDE_INDEX = 0;
        private const int DIG_LEFT_SIDE_INDEX = 1;
        private const int DIG_RIGHT_SIDE_INDEX = 2;
        #endregion

        #region SEQUENCE
        private Sequence _scaleSequence;
        private Guid _scaleSequenceID;
        #endregion

        public void Init(Ai ai)
        {
            if (_ai == null)
            {
                _ai = ai;
                _animator = transform.GetChild(0).GetComponent<Animator>();
                _animationEventListener = transform.GetChild(0).GetComponent<AiAnimationEventListener>();
                _animationEventListener.Init(this);
            }

            _animator.SetFloat(_scaleRateID, 0f);
            UpdateDigSpeed();
            UpdatePushSpeed();
            Land();

            AiEvents.OnSetCurrentPickaxeSpeed += UpdateDigSpeed;
            AiEvents.OnSetCurrentPickaxeSpeed += UpdatePushSpeed;

            AiEvents.OnMove += Move;
            AiEvents.OnIdle += Idle;
            AiEvents.OnDie += Die;
            AiEvents.OnWin += Win;
            AiEvents.OnLose += Lose;
            AiEvents.OnFly += Fly;
            AiEvents.OnFall += Fall;
            AiEvents.OnLand += Land;
            AiEvents.OnStartDigging += StartDigging;
            AiEvents.OnStopDigging += StopDigging;
            AiEvents.OnStagger += Stagger;
            AiEvents.OnStartPushing += StartPushing;
            AiEvents.OnStopPushing += StopPushing;
        }

        private void OnDisable()
        {
            if (_ai == null) return;

            AiEvents.OnSetCurrentPickaxeSpeed -= UpdateDigSpeed;
            AiEvents.OnSetCurrentPickaxeSpeed -= UpdatePushSpeed;

            AiEvents.OnMove -= Move;
            AiEvents.OnIdle -= Idle;
            AiEvents.OnDie -= Die;
            AiEvents.OnWin -= Win;
            AiEvents.OnLose -= Lose;
            AiEvents.OnFly -= Fly;
            AiEvents.OnFall -= Fall;
            AiEvents.OnLand -= Land;
            AiEvents.OnStartDigging -= StartDigging;
            AiEvents.OnStopDigging -= StopDigging;
            AiEvents.OnStagger -= Stagger;
            AiEvents.OnStartPushing -= StartPushing;
            AiEvents.OnStopPushing -= StopPushing;
        }

        #region BASIC ANIM FUNCTIONS
        private void Idle() => _animator.SetBool(_moveID, false);
        private void Move() => _animator.SetBool(_moveID, true);
        private void Die() => _animator.SetTrigger(_dieID);
        private void SelectRandomWin() => _animator.SetInteger(_cheerIndexID, Random.Range(1, 5));
        private void Win()
        {
            SelectRandomWin();
            _animator.SetTrigger(_winID);
        }
        private void SelectRandomLose() => _animator.SetInteger(_loseIndexID, Random.Range(1, 4));
        private void Lose()
        {
            SelectRandomLose();
            _animator.SetTrigger(_loseID);
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void Fly()
        {
            _animator.SetBool(_groundedID, false);
            _animator.SetBool(_flyingID, true);
        }
        private void Fall()
        {
            CheckForHeight();
            _animator.SetBool(_groundedID, false);
            _animator.SetBool(_flyingID, false);
        }
        private void Land() => _animator.SetBool(_groundedID, true);
        private void StartDigging()
        {
            _animator.SetInteger(_digSideIndexID, (int)_ai.DigHandler.CurrentBoxTriggerDirection);
            _animator.SetBool(_diggingID, true);
        }
        private void StopDigging() => _animator.SetBool(_diggingID, false);
        private void Stagger() => _animator.SetTrigger(_staggerID);
        private void UpdateDigSpeed() => _animator.SetFloat(_digSpeedID, DataManager.PickaxeSpeed + (DataManager.PickaxeSpeed * _ai.PowerUpHandler.SpeedRate));
        private void UpdatePushSpeed()
        {
            _animator.SetFloat(_pushSpeedRateID, 2f + (1f * _ai.PowerUpHandler.SpeedRate));
            _animator.SetFloat(_kickSpeedRateID, 1f + (1f * _ai.PowerUpHandler.SpeedRate));
        }
        private void StartPushing()
        {
            _animator.SetInteger(_digSideIndexID, (int)_ai.PushHandler.CurrentBoxTriggerDirection);
            _animator.SetBool(_pushingID, true);
            //_animator.applyRootMotion = true;
        }
        private void StopPushing() => _animator.SetBool(_pushingID, false);
        #endregion

        #region HELPERS
        private void CheckForHeight() => _animator.SetBool(_tooHighID, _ai.IsTooHigh);
        #endregion

        #region PUBLICS
        public void StartScaleSequence(float duration)
        {
            DeleteScaleSequence();
            CreateScaleSequence(duration);
            _scaleSequence.Play();
        }
        public void SelectPushOrKick() => _animator.SetBool(_kickingID, _ai.PushHandler.CurrentPushedBox.RightIsMiddleBox || _ai.PushHandler.CurrentPushedBox.LeftIsBorderBox);
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateScaleSequence(float duration)
        {
            if (_scaleSequence == null)
            {
                _scaleSequence = DOTween.Sequence();
                _scaleSequenceID = Guid.NewGuid();
                _scaleSequence.id = _scaleSequenceID;

                _scaleSequence.Append(DOVirtual.Float(2f, 0f, duration, r => {
                    _animator.SetFloat(_scaleRateID, r);
                }))
                    .OnComplete(() => {
                        _animator.SetFloat(_scaleRateID, 0f);
                        DeleteScaleSequence();
                    });
            }
        }
        private void DeleteScaleSequence()
        {
            DOTween.Kill(_scaleSequenceID);
            _scaleSequence = null;
        }
        #endregion
    }
}
