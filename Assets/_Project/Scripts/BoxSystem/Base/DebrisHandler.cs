using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class DebrisHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float _releaseForce = 10f;

        #region PROPERTIES
        public DebrisContainer DebrisContainer { get; private set; }
        public float ReleaseForce => _releaseForce;
        public BreakableBox BreakableBox { get; private set; }
        #endregion

        public void Init(BreakableBox breakableBox)
        {
            if (BreakableBox == null)
                BreakableBox = breakableBox;
        }

        public void ActivateDebrises()
        {
            SpawnRelevantDebris();

            DebrisContainer.Init(this);
        }

        private void SpawnRelevantDebris()
        {
            if (BreakableBox.BoxType == Enums.BoxType.Stone)
                DebrisContainer = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Debris_Stone, Vector3.zero, Quaternion.identity).GetComponent<DebrisContainer>();
            else if (BreakableBox.BoxType == Enums.BoxType.Copper)
                DebrisContainer = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Debris_Copper, Vector3.zero, Quaternion.identity).GetComponent<DebrisContainer>();
            else if (BreakableBox.BoxType == Enums.BoxType.Diamond)
                DebrisContainer = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Debris_Diamond, Vector3.zero, Quaternion.identity).GetComponent<DebrisContainer>();
        }
    }
}
