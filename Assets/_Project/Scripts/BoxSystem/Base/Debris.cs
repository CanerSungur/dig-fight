using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class Debris : MonoBehaviour
    {
        #region COMPONENTS
        private DebrisHandler _debrisHandler;
        private Rigidbody _rigidbody;
        private Material _crackMaterial;
        #endregion

        #region COLOR SEQUENCE
        private Sequence _colorSequence;
        private Guid _colorSequenceID;

        private Color _currentColor;
        private const float COLOR_CHANGE_DURATION = 1f;
        private const float MIN_OFFSET = -0.1f;
        private const float MAX_OFFSET = 0.1f;
        #endregion

        #region DISPOSE SEQUENCE
        private Sequence _disposeSequence;
        private Guid _disposeSequenceID;
        #endregion

        public void Init(DebrisHandler debrisHandler)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
                _debrisHandler = debrisHandler;
                _crackMaterial = GetComponent<MeshRenderer>().materials[1];
            }

            _rigidbody.isKinematic = true;
            _currentColor = new Color(1f, 1f, 1f, 0f);
            _crackMaterial.color = _currentColor;
            SetRandomCrackOffset();
        }

        #region PUBLICS
        public void Release()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(1f, 5f), Random.Range(1f, 2f)) * _debrisHandler.ReleaseForce, ForceMode.Impulse);
            DestroyAfterDelay();
        }
        public void MakeCrackBigger() => StartColorSequence(_debrisHandler.BreakableBox.GetCurrentHealthNormalized());
        #endregion

        #region PRIVATES
        private void DestroyAfterDelay()
        {
            // apply dispose sequence
            Destroy(gameObject, 5f);
        }
        private void SetRandomCrackOffset() => _crackMaterial.SetTextureOffset("_MainTex", new Vector2(Random.Range(MIN_OFFSET, MAX_OFFSET), Random.Range(MIN_OFFSET, MAX_OFFSET)));
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartColorSequence(float alpha)
        {
            DeleteColorSequence();
            CreateColorSequence(alpha);
            _colorSequence.Play();
        }
        private void CreateColorSequence(float alpha)
        {
            if (_colorSequence == null)
            {
                _colorSequence = DOTween.Sequence();
                _colorSequenceID = Guid.NewGuid();
                _colorSequence.id = _colorSequenceID;

                _colorSequence.Append(DOVirtual.Color(_currentColor, new Color(1f, 1f, 1f, alpha), COLOR_CHANGE_DURATION, r =>
                {
                    _currentColor = r;
                    _crackMaterial.color = _currentColor;
                })).OnComplete(DeleteColorSequence);
            }
        }
        private void DeleteColorSequence()
        {
            DOTween.Kill(_colorSequenceID);
            _colorSequence = null;
        }
        #endregion
    }
}
