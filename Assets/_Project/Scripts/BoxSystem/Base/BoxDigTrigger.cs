using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxDigTrigger : MonoBehaviour
    {
        private IBoxInteractable _interactableBox = null;
        //private PushableBox _pushableBox = null;
        //private BreakableBox _breakableBox = null;
        //private ExplosiveBox _explosiveBox = null;
        //private ChestBase _chest = null;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.BoxTriggerDirection triggerDirection;

        #region PROPERTIES
        public Enums.BoxTriggerDirection TriggerDirection => triggerDirection;
        #endregion

        private void OnEnable()
        {
            if (_interactableBox == null)
                _interactableBox = transform.parent.GetComponent<IBoxInteractable>();

            //if (_breakableBox == null)
            //    transform.parent.TryGetComponent(out _breakableBox);
            //if (_explosiveBox == null)
            //    transform.parent.TryGetComponent(out _explosiveBox);
            //if (_pushableBox == null)
            //    transform.parent.TryGetComponent(out _pushableBox);
            //if (_chest == null)
            //    transform.parent.TryGetComponent(out _chest);
        }

        #region PUBLICS
        public void AssignInteracter(Player player)
        {
            _interactableBox.AssignInteracter(player);

            //if (_breakableBox)
            //    _breakableBox.AssignHitter(player);
            //else if (_explosiveBox)
            //    _explosiveBox.AssignHitter(player);
            //else if (_pushableBox)
            //    _pushableBox.AssignPusher(player);
            //else if (_chest)
            //    _chest.AssignHitter(player);
        }
        #endregion
    }
}
