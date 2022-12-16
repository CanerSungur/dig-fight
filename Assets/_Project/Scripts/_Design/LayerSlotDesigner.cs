using UnityEngine;

namespace DigFight
{
    public class LayerSlotDesigner : MonoBehaviour
    {
        private GameObject _stoneBox, _copperBox, _diamondBox, _pushableBox, _explosiveBox, _chestSpeed, _chestPower, _chestDurability;

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
            InitializeBoxes();
            _chestSpeed.SetActive(true);
        }
        public void MakePowerChest()
        {
            InitializeBoxes();
            _chestPower.SetActive(true);
        }
        public void MakeDurabilityChest()
        {
            InitializeBoxes();
            _chestDurability.SetActive(true);
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

            _chestSpeed = transform.GetChild(5).gameObject;
            _chestSpeed.SetActive(false);
            _chestPower = transform.GetChild(6).gameObject;
            _chestPower.SetActive(false);
            _chestDurability = transform.GetChild(7).gameObject;
            _chestDurability.SetActive(false);
        }
    }
}
