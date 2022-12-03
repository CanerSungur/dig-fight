using UnityEngine;

namespace DigFight
{
    public class CrackHandler : MonoBehaviour
    {
        private BreakableBox _breakableBox;
        private Crack[] _cracks;

        #region PROPERTIES
        public BreakableBox BreakableBox => _breakableBox;
        #endregion

        public void Init(BreakableBox breakableBox)
        {
            if (_breakableBox == null)
            {
                _breakableBox = breakableBox;
                _cracks = GetComponentsInChildren<Crack>();
            }

            InitalizeCracks();
        }

        private void InitalizeCracks()
        {
            for (int i = 0; i < _cracks.Length; i++)
                _cracks[i].Init(this);
        }

        #region PUBLICS
        public void EnhanceCracks()
        {
            //Debug.Log("MAX: " + _breakableBox.MaxHealth);
            //Debug.Log("CURRENT: " + _breakableBox.CurrentHealth);
            for (int i = 0; i < _cracks.Length; i++)
                _cracks[i].Enhance(_breakableBox.GetCurrentHealthNormalized());

            _breakableBox.DebrisHandler.MakeCracksBigger();
            //Debug.Log(_breakableBox.GetCurrentHealthNormalized());
        }
        public void DisposeCracks()
        {
            for (int i = 0; i < _cracks.Length; i++)
                _cracks[i].Dispose();
        }
        #endregion
    }
}
