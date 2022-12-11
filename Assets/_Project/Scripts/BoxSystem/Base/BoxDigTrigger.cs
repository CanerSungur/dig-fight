using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxDigTrigger : MonoBehaviour
    {
        private PushableBox _pushableBox = null;
        private BreakableBox _breakableBox = null;
        private ExplosiveBox _explosiveBox = null;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxTriggerDirection triggerDirection;

        #region PROPERTIES
        public Enums.BoxTriggerDirection TriggerDirection => triggerDirection;
        #endregion

        private void OnEnable()
        {
            if (_breakableBox == null)
                transform.parent.TryGetComponent(out _breakableBox);
            if (_explosiveBox == null)
                transform.parent.TryGetComponent(out _explosiveBox);
            if (_pushableBox == null)
                transform.parent.TryGetComponent(out _pushableBox);
        }

        #region PUBLICS
        public void AssignInteracter(Player player)
        {
            if (_breakableBox)
                _breakableBox.AssignHitter(player);
            else if (_explosiveBox)
                _explosiveBox.AssignHitter(player);
            else if (_pushableBox)
                _pushableBox.AssignPusher(player);
        }
        #endregion
    }
}
