using System;
using UnityEngine;

namespace ZestGames
{
    public class PlayerEffectHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem stepParticle;
        [SerializeField] private ParticleSystem[] flyParticles;
        [SerializeField] private ParticleSystem[] boostParticles;

        public void Init(Player player)
        {
            stepParticle.Stop();
            StopFlyParticles();

            PlayerEvents.OnMove += StartedMoving;
            PlayerEvents.OnIdle += StoppedMoving;
            PlayerEvents.OnFly += StartedFlying;
            PlayerEvents.OnFall += StoppedFlying;
            PlayerEvents.OnTakePickaxeDurability += DurabilityPickup;
            PlayerEvents.OnTakePickaxeSpeed += SpeedPickup;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= StartedMoving;
            PlayerEvents.OnIdle -= StoppedMoving;
            PlayerEvents.OnFly -= StartedFlying;
            PlayerEvents.OnFall -= StoppedFlying;
            PlayerEvents.OnTakePickaxeDurability -= DurabilityPickup;
            PlayerEvents.OnTakePickaxeSpeed -= SpeedPickup;
        }

        #region EVENT HANDLER FUNCTIONS
        private void DurabilityPickup(int ignoreThis)
        {
            PlayBoostParticles();
        }
        private void SpeedPickup(float ignoreThis)
        {
            PlayBoostParticles();
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
        private void PlayBoostParticles()
        {
            for (int i = 0; i < boostParticles.Length; i++)
                boostParticles[i].Play();
        }
        #endregion
    }
}
