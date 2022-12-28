using DG.Tweening;
using UnityEngine;
using ZestGames;
using System;
using ZestCore.Utility;

namespace DigFight
{
    public class PushableBox : MonoBehaviour, IBoxInteractable
    {
        private Layer _layer;
        private Player _player;
        private Ai _ai;
        private Transform _meshTransform;

        #region MOVE SEQUENCE
        private Sequence _moveSequence, _enterScreenSequence, _leaveScreenSequence;
        private Guid _moveSequenceID, _enterScreenSequenceID, _leaveScreenSequenceID;
        private bool _moveSeqIsPlaying, _enterScreenSeqIsPlaying, _leaveScreenSeqIsPlaying = false;
        private const float BOX_LENGTH = 2f;
        private const float MID_MOVEMENT_LENGTH = 2.5f;
        //private const float MOVE_DURATION = 3f;
        #endregion

        [SerializeField] private LayerMask hittableLayers;
        private const string MIDDLE_BOX_LAYER = "MiddleBox";
        private const string BORDER_BOX_LAYER = "BorderBox";
        private const string BOX_TRIGGER_LAYER = "BoxTrigger;";
        private bool _rightIsMiddleBox, _rightIsBorderBox, _leftIsMiddleBox, _leftIsBorderBox;

        #region PROPERTIES
        public bool RightIsMiddleBox => _rightIsMiddleBox;
        public bool LeftIsBorderBox => _leftIsBorderBox;
        public bool IsReadyForPushing => /*!(_moveSeqIsPlaying && _enterScreenSeqIsPlaying && _leaveScreenSeqIsPlaying) && (_player || _ai) && */!_layer.IsBeingPushed;
        public Layer Layer => _layer;
        #endregion

        #region INTERFACE FUNCTIONS
        public void Init(Layer layer)
        {
            if (_layer == null)
            {
                _layer = layer;
                _meshTransform = transform.GetChild(0);
            }

            _layer.AddPushableBox(this);
            _leftIsMiddleBox = _leftIsBorderBox = _rightIsMiddleBox = _rightIsBorderBox = false;
            Delayer.DoActionAfterDelay(this, .5f, CheckSurroundings);
        }
        public void ChangeParent(Transform transform) => this.transform.SetParent(transform);
        public void AssignInteracter(Player player) => _player = player;
        public void AssignInteracter(Ai ai) => _ai = ai;
        #endregion

        #region PUBLICS 
        public void GetPushed(Enums.BoxTriggerDirection pushDirection)
        {
            if (pushDirection == Enums.BoxTriggerDirection.Left)
                _layer.PushBoxesLeft();
            else if (pushDirection == Enums.BoxTriggerDirection.Right)
                _layer.PushBoxesRight();
            else
            {
                Debug.Log("Unknown push direction!");
                return;
            }

            AudioManager.PlayAudioLoop(Enums.AudioType.PushBox);
            HapticEvents.OnPlayPush?.Invoke();

            //CameraManager.OnPushBackCameraForAWhile?.Invoke();
        }
        public void StartMoveSequence(Enums.BoxTriggerDirection pushDirection)
        {
            CheckSurroundings();

            if (pushDirection == Enums.BoxTriggerDirection.Left && _leftIsBorderBox)
            {
                DeleteMoveSequence();
                DeleteEnterScreenSequence();
                CreateLeaveScreenSequence(pushDirection);
                _leaveScreenSequence.Play();
            }
            else if (pushDirection == Enums.BoxTriggerDirection.Right && _rightIsBorderBox)
            {
                DeleteMoveSequence();
                DeleteEnterScreenSequence();
                CreateLeaveScreenSequence(pushDirection);
                _leaveScreenSequence.Play();
            }
            else
            {
                DeleteLeaveScreenSequence();
                DeleteEnterScreenSequence();
                CreateMoveSequence(pushDirection);
                _moveSequence.Play();
            }
        }
        #endregion

        #region HELPERS
        public void CheckSurroundings()
        {
            #region RIGHT RAYCAST
            Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, 2f, hittableLayers, QueryTriggerInteraction.Ignore);
            if (hitRight.transform != null)
            {
                _rightIsMiddleBox = hitRight.transform.gameObject.layer == LayerMask.NameToLayer(MIDDLE_BOX_LAYER);
                _rightIsBorderBox = hitRight.transform.gameObject.layer == LayerMask.NameToLayer(BORDER_BOX_LAYER);
            }
            else _rightIsMiddleBox = _rightIsBorderBox = false;
            #endregion

            #region LEFT RAYCAST
            Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, 2f, hittableLayers, QueryTriggerInteraction.Ignore);
            if (hitLeft.transform != null)
            {
                _leftIsMiddleBox = hitLeft.transform.gameObject.layer == LayerMask.NameToLayer(MIDDLE_BOX_LAYER);
                _leftIsBorderBox = hitLeft.transform.gameObject.layer == LayerMask.NameToLayer(BORDER_BOX_LAYER);
            }
            else _leftIsBorderBox = _leftIsMiddleBox = false;
            #endregion
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateLeaveScreenSequence(Enums.BoxTriggerDirection pushDirection)
        {
            if (_leaveScreenSequence == null)
            {
                _leaveScreenSeqIsPlaying = true;

                _leaveScreenSequence = DOTween.Sequence();
                _leaveScreenSequenceID = Guid.NewGuid();
                _leaveScreenSequence.id = _leaveScreenSequenceID;

                float targetPosX = pushDirection == Enums.BoxTriggerDirection.Left ? -_layer.BoxGap : (_layer.BoxCount) * 2 * _layer.BoxGap;
                _leaveScreenSequence.Append(transform.DOLocalMoveX(targetPosX, PlayerPushHandler.PushDuration * 0.2f))
                    .Join(_meshTransform.DOScale(Vector3.zero, PlayerPushHandler.PushDuration * 0.2f))
                    .OnComplete(() =>
                    {
                        CreateEnterScreenSequence(pushDirection);
                        _enterScreenSequence.Play();
                        DeleteLeaveScreenSequence();
                    });
            }
        }
        private void DeleteLeaveScreenSequence()
        {
            _leaveScreenSeqIsPlaying = false;
            DOTween.Kill(_leaveScreenSequenceID);
            _leaveScreenSequence = null;
        }
        // ########################
        private void CreateEnterScreenSequence(Enums.BoxTriggerDirection pushDirection)
        {
            if (_enterScreenSequence == null)
            {
                _enterScreenSeqIsPlaying = true;

                _enterScreenSequence = DOTween.Sequence();
                _enterScreenSequenceID = Guid.NewGuid();
                _enterScreenSequence.id = _enterScreenSequenceID;

                float targetPosX = pushDirection == Enums.BoxTriggerDirection.Left ? _layer.BoxCount * 2 * _layer.BoxGap : 0;
                float lastJumpPosX = pushDirection == Enums.BoxTriggerDirection.Left ? (_layer.BoxCount) * 2 * _layer.BoxGap : -_layer.BoxGap;

                transform.localPosition = new Vector3(lastJumpPosX, transform.localPosition.y, transform.localPosition.z);
                _enterScreenSequence.Append(transform.DOLocalMoveX(targetPosX, PlayerPushHandler.PushDuration * 0.8f)).Join(_meshTransform.DOScale(Vector3.one, PlayerPushHandler.PushDuration * 0.8f))
                    .Append(transform.DOShakeScale(1f, .25f))
                    .OnComplete(() =>
                    {
                        CheckSurroundings();
                        DeleteEnterScreenSequence();
                    });
            }
        }
        private void DeleteEnterScreenSequence()
        {
            _enterScreenSeqIsPlaying = false;
            DOTween.Kill(_enterScreenSequenceID);
            _enterScreenSequence = null;
        }
        // ##########################
        private void CreateMoveSequence(Enums.BoxTriggerDirection pushDirection)
        {
            if (_moveSequence == null)
            {
                _moveSeqIsPlaying = true;

                _moveSequence = DOTween.Sequence();
                _moveSequenceID = Guid.NewGuid();
                _moveSequence.id = _moveSequenceID;

                float targetPosX;
                Vector3 rotation;

                if (pushDirection == Enums.BoxTriggerDirection.Left)
                {
                    rotation = new Vector3(0f, 0f, 5f);

                    if (_leftIsMiddleBox) targetPosX = transform.localPosition.x - MID_MOVEMENT_LENGTH;
                    else targetPosX = transform.localPosition.x - BOX_LENGTH;
                }
                else
                {
                    rotation = new Vector3(0f, 0f, -5f);

                    if (_rightIsMiddleBox) targetPosX = transform.localPosition.x + MID_MOVEMENT_LENGTH;
                    else targetPosX = transform.localPosition.x + BOX_LENGTH;
                }

                _moveSequence.Append(transform.DOLocalMoveX(targetPosX, PlayerPushHandler.PushDuration))
                    .Join(transform.DOLocalRotate(rotation, 1f))
                    .Append(transform.DOShakeScale(1f, .25f))
                    .Join(transform.DOLocalRotate(Vector3.zero, 0.5f))
                    .OnComplete(() =>
                    {
                        CheckSurroundings();
                        DeleteMoveSequence();
                    });
            }
        }
        private void DeleteMoveSequence()
        {
            _moveSeqIsPlaying = false;
            DOTween.Kill(_moveSequenceID);
            _moveSequence = null;
        }
        #endregion
    }
}
