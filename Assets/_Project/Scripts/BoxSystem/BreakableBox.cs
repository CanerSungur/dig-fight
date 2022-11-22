using DG.Tweening;
using UnityEngine;
using ZestGames;
using System;

namespace DigFight
{
    public class BreakableBox : MonoBehaviour, IHealth, IDamageable
    {
        //[Header("-- SETUP --")]
        //[SerializeField] private ParticleSystem hitSmokePS;

        [Header("-- SETUP --")]
        [SerializeField] private int hp = 1;

        private Player _player;
        private Transform _meshTransform;

        //private const int DEFAULT_HP = 1;

        #region PROPERTIES
        public int MaxHealth => hp;
        public int CurrentHealth { get; set; }
        #endregion

        #region SEQUENCE
        private Sequence _shakeSequence;
        private Guid _shakeSequenceID;
        private const float SHAKE_DURATION = 0.5f;
        #endregion

        public void Init(Layer layer)
        {
            _meshTransform = transform.GetChild(0);
            CurrentHealth = MaxHealth;

            //if (layer.LayerNumber != 0)
            //    CurrentHealth *= layer.LayerNumber;
        }

        #region INTERFACE FUNCTIONS
        public void GetDamaged(int amount)
        {
            CameraManager.OnBoxHitShake?.Invoke();
            //hitSmokePS.Play();
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmokeSquare, transform.position, Quaternion.identity);
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxSmoke, transform.position + new Vector3(0f, 1f, -1f), Quaternion.identity);

            CurrentHealth -= amount;
            AudioManager.PlayAudio(Enums.AudioType.HitBox, 0.5f);

            if (CurrentHealth <= 0)
                Break();

            StartShakeSequence();
        }
        public void Break()
        {
            if (_player == null)
                Debug.LogWarning("Player is not assigned!");
            else
            {
                _player.StoppedDigging();
                _player.DigHandler.StopDiggingProcess();
            }

            gameObject.SetActive(false);
            AudioManager.PlayAudio(Enums.AudioType.BreakBox, 0.3f);
            CameraManager.OnBoxBreakShake?.Invoke();
        }
        #endregion

        #region PUBLICS
        public void AssignHitter(Player player)
        {
            _player = player;
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartShakeSequence()
        {
            CreateShakeSequence();
            _shakeSequence.Play();
        }
        private void CreateShakeSequence()
        {
            if (_shakeSequence == null)
            {
                _shakeSequence = DOTween.Sequence();
                _shakeSequenceID = Guid.NewGuid();
                _shakeSequence.id = _shakeSequenceID;

                _shakeSequence.Append(_meshTransform.DOShakeScale(SHAKE_DURATION, 0.25f))
                    .Join(_meshTransform.DOShakeRotation(SHAKE_DURATION, 5f))
                    .OnComplete(DeleteShakeSequence);
            }
        }
        private void DeleteShakeSequence()
        {
            DOTween.Kill(_shakeSequenceID);
            _shakeSequence = null;
        }
        #endregion
    }
}
