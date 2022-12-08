using UnityEngine;
using UnityEditor;

namespace DigFight
{
    public class LayerDesigner : MonoBehaviour
    {
        [Header("-- SPAWN SETUP --")]
        [SerializeField] private int _totalLayerCount = 10;

        [Space(20f)]
        [Header("-- PREFAB SETUP --")]
        [SerializeField] private GameObject firstLayer;
        [SerializeField] private GameObject lastLayer;
        [SerializeField] private GameObject middleLayer;

        private const float DISTANCE = 2.25f;
        private int _currentLayerCount = 0;

        private GameObject _boxContainer = null;

        #region PUBLICS
        public void SpawnLayers()
        {
            _currentLayerCount = 0;
            SpawnBoxContainer();

            for (int i = 0; i < _totalLayerCount; i++)
            {
                GameObject layer;
                _currentLayerCount++;

                if (_currentLayerCount == 1)
                    layer = PrefabUtility.InstantiatePrefab(firstLayer) as GameObject;
                else if (_currentLayerCount == _totalLayerCount)
                    layer = PrefabUtility.InstantiatePrefab(lastLayer) as GameObject;
                else
                    layer = PrefabUtility.InstantiatePrefab(middleLayer) as GameObject;

                layer.transform.SetParent(_boxContainer.transform);
                layer.transform.localPosition = new Vector3(0f, (_currentLayerCount - 1) * -DISTANCE, 0f);
            }
        }
        #endregion

        private void SpawnBoxContainer()
        {
            ClearBoxContainer();

            _boxContainer = new GameObject("BoxContainer", typeof(LayerHandler));
            GameObject borderContainer = new GameObject("BorderContainer");
            borderContainer.transform.SetParent(_boxContainer.transform);
            _boxContainer.transform.position = new Vector3(0f, -1f, 0f);
        }
        private void ClearBoxContainer()
        {
            if (_boxContainer != null)
            {
                DestroyImmediate(_boxContainer);
                _boxContainer = null;
            }
        }
    }
}
