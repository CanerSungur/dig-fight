using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;

namespace DigFight
{
    public class DebrisHandler : MonoBehaviour
    {
        [SerializeField] private List<Debris> _debris;
        [SerializeField] private float _releaseForce = 10f;

        private int _totalDebrisCount;
        private int _currentDebrisCount;

        #region PROPERTIES
        public int TotalDebrisCount => _totalDebrisCount;
        public float ReleaseForce => _releaseForce;
        public BreakableBox BreakableBox { get; private set; }
        #endregion

        public void Init(BreakableBox breakableBox)
        {
            if (BreakableBox == null)
                BreakableBox = breakableBox;

            RNG.ShuffleList(_debris);
            _currentDebrisCount = _totalDebrisCount = _debris.Count;

            for (int i = 0; i < _debris.Count; i++)
                _debris[i].Init(this);
        }

        #region PUBLICS
        public void ReleaseDebris(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _debris[0].transform.SetParent(null);
                _debris[0].Release();
                RemoveDebris(_debris[0]);
                _totalDebrisCount--;
            }
        }
        public void MakeCracksBigger()
        {
            for (int i = 0; i < _debris.Count; i++)
                _debris[i].MakeCrackBigger();
        }
        #endregion

        private void RemoveDebris(Debris debris)
        {
            if (_debris.Contains(debris))
                _debris.Remove(debris);
        }
    }
}
