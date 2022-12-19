using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class AiAnimationEventListener : MonoBehaviour
    {
        private AiAnimationController _animationController;

        public void Init(AiAnimationController animationController)
        {
            if (_animationController == null)
                _animationController = animationController;
        }

        #region ANIMATION EVENT FUNCTIONS
        public void AlertObservers(string message)
        {
            if (message.Equals("DigMotionEnded"))
            {
                _animationController.Ai.StoppedDigging();
                //if ((int)_animationController.Ai.InputHandler.DigDirection == (int)_animationController.Ai.DigHandler.CurrentBoxTriggerDirection)
                //    _animationController.Ai.DigHandler.StartDiggingProcess(_animationController.Ai.DigHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("EnableCanHit"))
            {
                _animationController.Ai.DigHandler.Pickaxe.OnCanHit?.Invoke();
                //AudioEvents.OnPlaySwing?.Invoke();
            }
            else if (message.Equals("DisableCanHit"))
                _animationController.Ai.DigHandler.Pickaxe.OnCannotHit?.Invoke();
            else if (message.Equals("SwingAnimStarted"))
            {
                AiAudioEvents.OnPlaySwing?.Invoke();

            }
            else if (message.Equals("PushNow"))
            {
                if (!_animationController.Ai.PushHandler.CurrentPushedBox.IsReadyForPushing) return;

                _animationController.Ai.PushHandler.CurrentPushedBox.GetPushed(_animationController.Ai.PushHandler.CurrentBoxTriggerDirection);

                if (!_animationController.Ai.PushHandler.CurrentPushedBox.RightIsMiddleBox && !_animationController.Ai.PushHandler.CurrentPushedBox.LeftIsBorderBox)
                    _animationController.Ai.StartPushSequence(_animationController.Ai.PushHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("HopAfterKick"))
            {
                _animationController.Ai.StartKickSequence(_animationController.Ai.PushHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("PushFinished"))
            {
                AudioManager.StopAudioLoop();

                _animationController.Ai.StoppedPushing();
                _animationController.Ai.PushHandler.StopPushingProcess();
                //_animationController.Animator.applyRootMotion = false;

                AudioManager.PlayAudio(Enums.AudioType.PushBoxDrop);
            }
            else if (message.Equals("KickFinished"))
            {
                Delayer.DoActionAfterDelay(this, 1.2f, () => {
                    AudioManager.StopAudioLoop();

                    _animationController.Ai.StoppedPushing();
                    _animationController.Ai.PushHandler.StopPushingProcess();
                    //_animationController.Animator.applyRootMotion = false;

                    AudioManager.PlayAudio(Enums.AudioType.PushBoxDrop);
                });
            }
        }
        #endregion
    }
}
