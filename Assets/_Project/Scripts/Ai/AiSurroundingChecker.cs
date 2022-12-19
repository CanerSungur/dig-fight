using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class AiSurroundingChecker : MonoBehaviour
    {
        private Enums.Surrounding _left, _right, _up, _down;

        [Header("-- SETUP --")]
        [SerializeField] private LayerMask _hittableLayers;

        #region PROPERTIES
        public Enums.Surrounding Left => _left;
        public Enums.Surrounding Right => _right;
        public Enums.Surrounding Up => _up;
        public Enums.Surrounding Down => _down;
        #endregion

        #region CONTROLS
        public bool CanFly => _up == Enums.Surrounding.Empty;
        public bool CanRun => _left == Enums.Surrounding.Empty || _right == Enums.Surrounding.Empty;
        public bool CanDig => _left == Enums.Surrounding.BreakableBox || _left == Enums.Surrounding.ExplosiveBox || _left == Enums.Surrounding.Chest
            || _right == Enums.Surrounding.BreakableBox || _right == Enums.Surrounding.ExplosiveBox || _right == Enums.Surrounding.Chest
            || _down == Enums.Surrounding.BreakableBox || _down == Enums.Surrounding.ExplosiveBox || _down == Enums.Surrounding.Chest;
        public bool CanPush => _left == Enums.Surrounding.PushableBox || _right == Enums.Surrounding.PushableBox;
        #endregion

        public void Init(Ai ai)
        {

        }

        private void Update()
        {
            CheckSurroundings();
        }

        public void CheckSurroundings()
        {
            _left = Check(transform.right);
            _right = Check(-transform.right);
            _up = Check(transform.up);
            _down = Check(-transform.up);
        }

        #region HELPERS
        private Enums.Surrounding Check(Vector3 direction)
        {
            if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, 2f, _hittableLayers, QueryTriggerInteraction.Ignore))
                return hit.transform.GetComponent<BreakableBox>() ? Enums.Surrounding.BreakableBox
                    : hit.transform.GetComponent<ExplosiveBox>() ? Enums.Surrounding.ExplosiveBox
                    : hit.transform.GetComponent<PushableBox>() ? Enums.Surrounding.PushableBox
                    : hit.transform.GetComponent<ChestBase>() ? Enums.Surrounding.Chest
                    : hit.transform.GetComponent<LevelEndBox>() ? Enums.Surrounding.FinishLine
                    : Enums.Surrounding.Wall;
            else
                return Enums.Surrounding.Empty;
        }
        #endregion
    }
}
