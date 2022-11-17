using UnityEngine;

namespace ZestGames
{
    public class PlayerEffectHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem stepParticle;
        [SerializeField] private ParticleSystem[] flyParticles;

        public void Init(Player player)
        {
            stepParticle.Stop();
            StopFlyParticles();

            PlayerEvents.OnMove += StartedMoving;
            PlayerEvents.OnIdle += StoppedMoving;
            PlayerEvents.OnFly += StartedFlying;
            PlayerEvents.OnFall += StoppedFlying;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= StartedMoving;
            PlayerEvents.OnIdle -= StoppedMoving;
            PlayerEvents.OnFly -= StartedFlying;
            PlayerEvents.OnFall -= StoppedFlying;
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
            {
                flyParticles[i].Play();
            }
        }
        private void StopFlyParticles()
        {
            for (int i = 0; i < flyParticles.Length; i++)
            {
                flyParticles[i].Stop();
            }
        }
        #endregion
    }
}
