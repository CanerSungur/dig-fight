using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxDigTrigger : MonoBehaviour
    {
        private IBoxInteractable _interactableBox = null;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxTriggerDirection triggerDirection;

        #region PROPERTIES
        public Enums.BoxTriggerDirection TriggerDirection => triggerDirection;
        #endregion

        private void OnEnable()
        {
            if (_interactableBox == null)
                _interactableBox = transform.parent.GetComponent<IBoxInteractable>();
        }

        #region PUBLICS
        public void AssignInteracter(Player player) => _interactableBox.AssignInteracter(player);
        public void AssignInteracter(Ai ai) => _interactableBox.AssignInteracter(ai);
        #endregion
    }
}
