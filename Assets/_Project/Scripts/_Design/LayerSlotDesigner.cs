using UnityEngine;

namespace DigFight
{
    public class LayerSlotDesigner : MonoBehaviour
    {
        private GameObject _stoneBox, _copperBox, _diamondBox, _pushableBox, _explosiveBox;

        #region PUBLICS
        public void MakeStoneBox()
        {
            InitializeBoxes();
            _stoneBox.SetActive(true);   
        }
        public void MakeCopperBox()
        {
            InitializeBoxes();
            _copperBox.SetActive(true);
        }
        public void MakeDiamondBox()
        {
            InitializeBoxes();
            _diamondBox.SetActive(true);
        }

        public void MakeExplosiveBox()
        {
            InitializeBoxes();
            _explosiveBox.SetActive(true);
        }

        public void MakePushableBox()
        {
            InitializeBoxes();
            _pushableBox.SetActive(true);
        }
        public void MakeSpeedChest()
        {

        }
        public void MakePowerChest()
        {

        }
        public void MakeDurabilityChest()
        {

        }
        #endregion

        private void InitializeBoxes()
        {
            _stoneBox = transform.GetChild(0).gameObject;
            _stoneBox.SetActive(false);
            _copperBox = transform.GetChild(1).gameObject;
            _copperBox.SetActive(false);
            _diamondBox = transform.GetChild(2).gameObject;
            _diamondBox.SetActive(false);

            _pushableBox = transform.GetChild(3).gameObject;
            _pushableBox.SetActive(false);
            
            _explosiveBox = transform.GetChild(4).gameObject;
            _explosiveBox.SetActive(false);
        }
    }
}
