using UnityEngine;
using UnityEngine.UI;
using System;
using ZestCore.Utility;

namespace ZestGames
{
    /// <summary>
    /// Plays sound and animation first, then executes the action given.
    /// Give the action as a parameter of CustomButton's event.
    /// </summary>
    public class CustomButton : Button
    {
        private Animator _animator;
        private Animation _anim;
        private float _animationDuration = 0.5f;
        public event Action<Action> OnClicked;

        protected override void OnEnable()
        {
            TryGetComponent(out _animator);
            TryGetComponent(out _anim);
            if (!_anim && !_animator)
                _animationDuration = 0f;

            OnClicked += Clicked;
        }

        protected override void OnDisable()
        {
            OnClicked -= Clicked;
        }

        private void Clicked(Action action)
        {
            // Play audio
            AudioManager.PlayAudio(Enums.AudioType.Button_Click);

            // Reset and Play the animation
            if (_anim)
            {
                _anim.Rewind();
                _anim.Play();
            }

            if (_animator)
                _animator.SetTrigger("Click");

            // Do the action with delay
            Delayer.DoActionAfterDelay(this, _animationDuration, () => action());
        }

        public void TriggerClick(Action action, Action simultaniousAction = null)
        {
            simultaniousAction?.Invoke();
            OnClicked?.Invoke(action);
        }
    }
}
