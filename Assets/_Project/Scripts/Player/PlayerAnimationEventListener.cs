using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class PlayerAnimationEventListener : MonoBehaviour
    {
        private PlayerAnimationController _animationController;

        public void Init(PlayerAnimationController animationController)
        {
            if (_animationController == null)
                _animationController = animationController;
        }

        #region ANIMATION EVENT FUNCTIONS
        public void AlertObservers(string message)
        {
            if (message.Equals("DigMotionEnded"))
            {
                _animationController.Player.StoppedDigging();
                if ((int)_animationController.Player.InputHandler.DigDirection == (int)_animationController.Player.DigHandler.CurrentBoxTriggerDirection)
                    _animationController.Player.DigHandler.StartDiggingProcess(_animationController.Player.DigHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("EnableCanHit"))
            {
                _animationController.Player.DigHandler.Pickaxe.OnCanHit?.Invoke();
                //AudioEvents.OnPlaySwing?.Invoke();
            }
            else if (message.Equals("DisableCanHit"))
                _animationController.Player.DigHandler.Pickaxe.OnCannotHit?.Invoke();
            else if (message.Equals("SwingAnimStarted"))
            {
                PlayerAudioEvents.OnPlaySwing?.Invoke();

            }
            else if (message.Equals("PushNow"))
            {
                if (!_animationController.Player.PushHandler.CurrentPushedBox.IsReadyForPushing) return;

                _animationController.Player.PushHandler.CurrentPushedBox.GetPushed(_animationController.Player.PushHandler.CurrentBoxTriggerDirection);

                if (!_animationController.Player.PushHandler.CurrentPushedBox.RightIsMiddleBox && !_animationController.Player.PushHandler.CurrentPushedBox.LeftIsBorderBox)
                    _animationController.Player.StartPushSequence(_animationController.Player.PushHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("HopAfterKick"))
            {
                _animationController.Player.StartKickSequence(_animationController.Player.PushHandler.CurrentBoxTriggerDirection);
            }
            else if (message.Equals("PushFinished"))
            {
                //if (_player.PushHandler.CurrentPushedBox)

                AudioManager.StopAudioLoop();

                _animationController.Player.StoppedPushing();
                _animationController.Player.PushHandler.StopPushingProcess();
                //_animationController.Animator.applyRootMotion = false;

                AudioManager.PlayAudio(Enums.AudioType.PushBoxDrop);
            }
            else if (message.Equals("KickFinished"))
            {
                Delayer.DoActionAfterDelay(this, 1.2f, () => {
                    AudioManager.StopAudioLoop();

                    _animationController.Player.StoppedPushing();
                    _animationController.Player.PushHandler.StopPushingProcess();
                    //_animationController.Animator.applyRootMotion = false;

                    AudioManager.PlayAudio(Enums.AudioType.PushBoxDrop);
                });
            }
        }
        #endregion
    }
}
