using System;
using UnityEngine;
using DigFight;
using DG.Tweening;

namespace ZestGames
{
    public class PlayerEffectHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem stepParticle;
        [SerializeField] private ParticleSystem[] flyParticles;
        [SerializeField] private ParticleSystem[] boostParticles;

        [Header("-- SPEED POWERUP SETUP --")]
        [SerializeField] private ParticleSystem _speedPowerupParticle;
        [SerializeField, ColorUsage(true, true)] private Color _speedColor;

        [Header("-- POWER POWERUP SETUP --")]
        [SerializeField] private ParticleSystem[] _powerPowerupParticles;
        [SerializeField, ColorUsage(true, true)] private Color _powerColor;

        [Space(20f)]
        [SerializeField, ColorUsage(true, true)] private Color _multiplePowerUpColor;

        #region MATERIAL CHANGE SECTION
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        //private Material _currentMaterial;
        private Sequence _enableSpecularSequence;
        private Guid _enableSpecularSequenceID;
        private const float SPECULAR_CHANGE_DURATION = 3f;
        #endregion

        private bool _powerPowerUpIsActive, _speedPowerUpIsActive = false;

        public void Init(Player player)
        {
            if (_skinnedMeshRenderer == null)
                _skinnedMeshRenderer = transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();
            DisableSpecular();

            stepParticle.Stop();
            StopFlyParticles();

            PlayerEvents.OnMove += StartedMoving;
            PlayerEvents.OnIdle += StoppedMoving;
            PlayerEvents.OnFly += StartedFlying;
            PlayerEvents.OnFall += StoppedFlying;
            PlayerEvents.OnActivatePickaxeDurability += DurabilityPickup;
            PlayerEvents.OnActivatePickaxeSpeed += SpeedPickup;
            PlayerEvents.OnActivatePickaxePower += PowerPickup;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= StartedMoving;
            PlayerEvents.OnIdle -= StoppedMoving;
            PlayerEvents.OnFly -= StartedFlying;
            PlayerEvents.OnFall -= StoppedFlying;
            PlayerEvents.OnActivatePickaxeDurability -= DurabilityPickup;
            PlayerEvents.OnActivatePickaxeSpeed -= SpeedPickup;
            PlayerEvents.OnActivatePickaxePower -= PowerPickup;
        }

        #region EVENT HANDLER FUNCTIONS
        private void DurabilityPickup(PowerUp ignoreThis)
        {
            //PlayerEvents.OnCheer?.Invoke();
            PlayPowerUpParticles();
        }
        private void SpeedPickup(PowerUp ignoreThis)
        {
            _speedPowerUpIsActive = true;

            //PlayerEvents.OnCheer?.Invoke();

            if (_powerPowerUpIsActive)
                ChangeSpecularColor(_multiplePowerUpColor);
            else
                ChangeSpecularColor(_speedColor);

            EnableSpecular();
            PlayPowerUpParticles();
            _speedPowerupParticle.Play();
        }
        private void PowerPickup(PowerUp ignoreThis)
        {
            _powerPowerUpIsActive = true;

            //PlayerEvents.OnCheer?.Invoke();

            if (_speedPowerUpIsActive)
                ChangeSpecularColor(_multiplePowerUpColor);
            else
                ChangeSpecularColor(_powerColor);

            EnableSpecular();
            PlayPowerUpParticles();
            PlayPowerPowerUpParticles();
        }
        private void StartedMoving()
        {
            stepParticle.Play();
            StopFlyParticles();
        }
        private void StoppedMoving()
        {
            stepParticle.Stop();
        }
        private void StartedFlying()
        {
            StartFlyParticles();
            stepParticle.Stop();
        }
        private void StoppedFlying()
        {
            StopFlyParticles();
        }
        #endregion

        #region HELPERS
        private void StartFlyParticles()
        {
            for (int i = 0; i < flyParticles.Length; i++)
                flyParticles[i].Play();
        }
        private void StopFlyParticles()
        {
            for (int i = 0; i < flyParticles.Length; i++)
                flyParticles[i].Stop();
        }
        private void PlayPowerUpParticles()
        {
            for (int i = 0; i < boostParticles.Length; i++)
                boostParticles[i].Play();
        }
        private void PlayPowerPowerUpParticles()
        {
            for (int i = 0; i < _powerPowerupParticles.Length; i++)
                _powerPowerupParticles[i].Play();
        }
        private void StopPowerPowerUpParticles()
        {
            for (int i = 0; i < _powerPowerupParticles.Length; i++)
                _powerPowerupParticles[i].Stop();
        }
        #endregion

        #region MATERIAL CHANGE FUNCTIONS
        private void ChangeSpecularColor(Color color) => _skinnedMeshRenderer.material.SetColor("_FlatSpecularColor", color);
        private void DisableSpecular() => _skinnedMeshRenderer.material.SetFloat("_FlatSpecularSize", 0f);
        private void EnableSpecular()
        {
            //_skinnedMeshRenderer.material.SetFloat("_SpecularEnabled", 1f);

            DeleteEnableSpecularSequence();
            CreateEnableSpecularSequence();
            _enableSpecularSequence.Play();
        }
        private void CreateEnableSpecularSequence()
        {
            if (_enableSpecularSequence == null)
            {
                _enableSpecularSequence = DOTween.Sequence();
                _enableSpecularSequenceID = Guid.NewGuid();
                _enableSpecularSequence.id = _enableSpecularSequenceID;
                
                _skinnedMeshRenderer.material.SetFloat("_FlatSpecularSize", 0f);
                _enableSpecularSequence.Append(DOVirtual.Float(0f, 1f, SPECULAR_CHANGE_DURATION, r => {
                    _skinnedMeshRenderer.material.SetFloat("_FlatSpecularSize", r);
                }))
                    .OnComplete(() => {
                        _skinnedMeshRenderer.material.SetFloat("_FlatSpecularSize", 1f);
                        DeleteEnableSpecularSequence();
                    });
            }
        }
        private void DeleteEnableSpecularSequence()
        {
            DOTween.Kill(_enableSpecularSequenceID);
            _enableSpecularSequence = null;
        }
        #endregion

        #region PUBLICS
        public void StopPickaxeSpeed()
        {
            _speedPowerUpIsActive = false;
            _speedPowerupParticle.Stop();

            if (_powerPowerUpIsActive)
                ChangeSpecularColor(_powerColor);
            else
                DisableSpecular();
        }
        public void StopPickaxePower()
        {
            _powerPowerUpIsActive = false;
            StopPowerPowerUpParticles();

            if (_speedPowerUpIsActive)
                ChangeSpecularColor(_speedColor);
            else
                DisableSpecular();
        }
        #endregion
    }
}
