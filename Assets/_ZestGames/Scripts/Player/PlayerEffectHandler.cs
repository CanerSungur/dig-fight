using UnityEngine;

namespace ZestGames
{
    public class PlayerEffectHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem stepParticle;

        public void Init(Player player)
        {
            stepParticle.Stop();

            PlayerEvents.OnMove += StartedMoving;
            PlayerEvents.OnIdle += StoppedMoving;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= StartedMoving;
            PlayerEvents.OnIdle -= StoppedMoving;
        }

        private void StartedMoving()
        {
            stepParticle.Play();
        }
        private void StoppedMoving()
        {
            stepParticle.Stop();
        }
    }
}
