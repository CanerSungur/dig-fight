using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class VFXVoxel : MonoBehaviour
    {
        private ParticleSystem _voxelPS;
        private ParticleSystem _voxelChildPS;
        private ParticleSystem.EmissionModule _voxelPSEmission;

        #region BURST RELATED
        private const int LOW_HIT_POWER = 5;
        private const int MEDIUM_HIT_POWER = 10;
        private const int HIGH_HIT_POWER = 20;
        private const float BURST_TIME = 0.1f;

        private readonly ParticleSystem.Burst _lowBurst = new ParticleSystem.Burst(BURST_TIME, LOW_HIT_POWER, 1, 0.01f);
        private readonly ParticleSystem.Burst _mediumBurst = new ParticleSystem.Burst(BURST_TIME, MEDIUM_HIT_POWER, 1, 0.01f);
        private readonly ParticleSystem.Burst _highBurst = new ParticleSystem.Burst(BURST_TIME, HIGH_HIT_POWER, 1, 0.01f);
        #endregion

        public void Init(Enums.HitPower affectedHitPower)
        {
            if (_voxelChildPS == null)
            {
                _voxelPS = GetComponent<ParticleSystem>();
                _voxelChildPS = transform.GetChild(1).GetComponent<ParticleSystem>();
                _voxelPSEmission = _voxelChildPS.emission;
            }

            SetVoxelEmission(affectedHitPower);
            _voxelPS.Play();
        }

        private void SetVoxelEmission(Enums.HitPower affectedHitPower)
        {
            if (affectedHitPower == Enums.HitPower.Low)
                _voxelPSEmission.SetBurst(0, _lowBurst);
            else if (affectedHitPower == Enums.HitPower.Medium)
                _voxelPSEmission.SetBurst(0, _mediumBurst);
            else if (affectedHitPower == Enums.HitPower.High)
                _voxelPSEmission.SetBurst(0, _highBurst);
            else
                Debug.Log("Unknown Affected Hit Power", this);
        }
    }
}
