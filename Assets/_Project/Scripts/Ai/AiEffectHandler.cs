using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiEffectHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem stepParticle;
        [SerializeField] private ParticleSystem[] flyParticles;

        public void Init(Ai ai)
        {
            stepParticle.Stop();
            StopFlyParticles();

            AiEvents.OnMove += StartedMoving;
            AiEvents.OnIdle += StoppedMoving;
            AiEvents.OnFly += StartedFlying;
            AiEvents.OnFall += StoppedFlying;
        }

        private void OnDisable()
        {
            AiEvents.OnMove -= StartedMoving;
            AiEvents.OnIdle -= StoppedMoving;
            AiEvents.OnFly -= StartedFlying;
            AiEvents.OnFall -= StoppedFlying;
        }

        #region EVENT HANDLER FUNCTIONS
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
        #endregion
    }
}
