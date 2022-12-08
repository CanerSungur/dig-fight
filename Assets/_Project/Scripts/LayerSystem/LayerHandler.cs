using UnityEngine;

namespace DigFight
{
    public class LayerHandler : MonoBehaviour
    {
        private Layer[] _layers;
        private Transform _borderContainer;

        #region PROPERTIES
        public Transform BorderContainer => _borderContainer;
        #endregion

        public static int TotalLayerCount { get; private set; }

        private void Awake()
        {
            if (_layers == null)
            {
                _layers = GetComponentsInChildren<Layer>();
                _borderContainer = transform.GetChild(0);
                TotalLayerCount = _layers.Length;

                InitializeLayers();
                CombineBorderBoxMeshes();
            }
        }

        #region HELPERS
        private void InitializeLayers()
        {
            for (int i = 0; i < _layers.Length; i++)
                _layers[i].Init(this);
        }
        private void SetParentAsBorderBoxContainer(Transform borderBoxTransform) => borderBoxTransform.SetParent(_borderContainer);
        private void CombineBorderBoxMeshes()
        {
            MeshCombiner meshCombiner = _borderContainer.gameObject.AddComponent<MeshCombiner>();
            meshCombiner.CreateMultiMaterialMesh = true;
            meshCombiner.DestroyCombinedChildren = true;
            meshCombiner.CombineMeshes(false);
        }
        #endregion
    }
}
