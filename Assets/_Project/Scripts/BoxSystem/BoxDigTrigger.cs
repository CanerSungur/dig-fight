using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxDigTrigger : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxTriggerDirection triggerDirection;

        #region PROPERTIES
        public Enums.BoxTriggerDirection TriggerDirection => triggerDirection;
        #endregion
    }
}
