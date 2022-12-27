using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ExplosiveBoxSkullCanvas : MonoBehaviour
    {
        private ExplosiveBox _explosiveBox;
        private bool _triggered;

        public void Init(ExplosiveBox explosiveBox)
        {
            if (_explosiveBox == null)
                _explosiveBox = explosiveBox;

            _triggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.TryGetComponent(out Player player) || other.TryGetComponent(out Ai ai)) && !_triggered)
            {
                _triggered = true;
                _explosiveBox.EnableSkull();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.TryGetComponent(out Player player) || other.TryGetComponent(out Ai ai)) && _triggered)
            {
                _triggered = false;
                _explosiveBox.DisableSkull();
            }
        }
    }
}
