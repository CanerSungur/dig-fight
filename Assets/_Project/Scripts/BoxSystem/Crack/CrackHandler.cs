using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class CrackHandler : MonoBehaviour
    {
        #region COMPONENTS
        private BreakableBox _breakableBox;
        private Crack _crack;
        private Material _crackMaterial;
        #endregion

        #region PROPERTIES
        public BreakableBox BreakableBox => _breakableBox;
        #endregion

        #region CRACK MATERIAL SEQUENCE
        private Sequence _crackMaterialSequence;
        private Guid _crackMaterialSequenceID;

        private Color _currentColor = new Color(1f, 1f, 1f, 0f);
        private const float CRACK_MATERIAL_CHANGE_DURATION = 1f;
        private const float MIN_OFFSET = -0.1f;
        private const float MAX_OFFSET = 0.1f;
        #endregion

        public void Init(BreakableBox breakableBox)
        {
            if (_breakableBox == null)
            {
                _breakableBox = breakableBox;
                _crack = GetComponentInChildren<Crack>();
                _crackMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().materials[1];
            }

            InitalizeCracks();

            _crackMaterial.color = _currentColor;
            SetRandomCrackOffset();
        }

        private void InitalizeCracks()
        {
            _crack.Init(this);
            //for (int i = 0; i < _crack.Length; i++)
            //    _crack[i].Init(this);
        }
        private void SetRandomCrackOffset() => _crackMaterial.mainTextureOffset = new Vector2(Random.Range(MIN_OFFSET, MAX_OFFSET), Random.Range(MIN_OFFSET, MAX_OFFSET));

        #region PUBLICS
        public void EnhanceCracks()
        {
            //for (int i = 0; i < _crack.Length; i++)
            //    _crack[i].Enhance(_breakableBox.GetCurrentHealthNormalized());

            _crack.Enhance(_breakableBox.GetCurrentHealthNormalized());
            StartCrackMaterialSequence(_breakableBox.GetCurrentHealthNormalized());
        }
        public void DisposeCracks()
        {
            _crack.Dispose();
            //for (int i = 0; i < _crack.Length; i++)
            //    _crack[i].Dispose();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartCrackMaterialSequence(float alpha)
        {
            DeleteCrackMaterialSequence();
            CreateCrackMaterialSequence(alpha);
            _crackMaterialSequence.Play();
        }
        private void CreateCrackMaterialSequence(float alpha)
        {
            if (_crackMaterialSequence == null)
            {
                _crackMaterialSequence = DOTween.Sequence();
                _crackMaterialSequenceID = Guid.NewGuid();
                _crackMaterialSequence.id = _crackMaterialSequenceID;

                _crackMaterialSequence.Append(DOVirtual.Color(_currentColor, new Color(1f, 1f, 1f, alpha), CRACK_MATERIAL_CHANGE_DURATION, r =>
                {
                    _currentColor = r;
                    _crackMaterial.color = _currentColor;
                })).OnComplete(DeleteCrackMaterialSequence);
            }
        }
        private void DeleteCrackMaterialSequence()
        {
            DOTween.Kill(_crackMaterialSequenceID);
            _crackMaterialSequence = null;
        }
#endregion
    }
}
