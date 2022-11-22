using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxDigTrigger : MonoBehaviour
    {
        private BreakableBox _box;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxTriggerDirection triggerDirection;

        #region PROPERTIES
        public Enums.BoxTriggerDirection TriggerDirection => triggerDirection;
        #endregion

        private void OnEnable()
        {
            if (_box == null)
                _box = GetComponentInParent<BreakableBox>();
        }

        #region PUBLICS
        public void AssignHitter(Player player)
        {
            _box.AssignHitter(player);
        }
        #endregion
    }
}
