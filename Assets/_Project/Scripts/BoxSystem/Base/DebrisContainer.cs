using UnityEngine;

namespace DigFight
{
    public class DebrisContainer : MonoBehaviour
    {
        private Debris[] _debrises;

        public DebrisHandler ActivaterDebrisHandler { get; private set; }

        public void Init(DebrisHandler debrisHandler)
        {
            if (_debrises == null)
                _debrises = GetComponentsInChildren<Debris>();

            ActivaterDebrisHandler = debrisHandler;
            transform.position = ActivaterDebrisHandler.transform.position;
            ReleaseDebrises();
        }

        private void ReleaseDebrises()
        {
            for (int i = 0; i < _debrises.Length; i++)
            {
                _debrises[i].Init(this);
                _debrises[i].Release();
            }
        }

        #region PUBLIC
        public void Dispose() => gameObject.SetActive(false);
        #endregion
    }
}
