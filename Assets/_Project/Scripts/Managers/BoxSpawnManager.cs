using System.Collections.Generic;
using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class BoxSpawnManager : MonoBehaviour
    {
        #region PREFAB DATA SECTION
        private PrefabSO[] _prefabData;
        private Dictionary<Enums.PrefabStamp, GameObject> _prefabDictionary;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private int layerCount = 5;
        [SerializeField] private Transform boxContainerTransform;

        #region SPAWN DATA
        private const int LAYER_BOX_COUNT = 5;
        private const float BOX_GAP = 2.25f;
        private int _currentLayerCount;
        #endregion

        #region PROPERTIES
        public Dictionary<Enums.PrefabStamp, GameObject> PrefabDictionary => _prefabDictionary;
        public Transform BoxContainerTransform => boxContainerTransform;
        #endregion

        public void Init(GameManager gameManager)
        {
            _prefabData = Resources.LoadAll<PrefabSO>("_PrefabData/");
            InitializePrefabDictionary();

            _currentLayerCount = 0;

            SpawnLayers();
        }

        private void InitializePrefabDictionary()
        {
            _prefabDictionary = new Dictionary<Enums.PrefabStamp, GameObject>();

            for (int i = 0; i < _prefabData.Length; i++)
            {
                if (!_prefabDictionary.ContainsKey(_prefabData[i].Stamp))
                    _prefabDictionary.Add(_prefabData[i].Stamp, _prefabData[i].Prefab);
            }
        }
        private void SpawnLayers()
        {
            for (int i = 0; i < layerCount; i++)
            {
                Layer layer = new GameObject($"Layer_{_currentLayerCount}", typeof(Layer)).GetComponent<Layer>();
                layer.Init(this, _currentLayerCount);

                _currentLayerCount++;

                //for (int j = 0; j < LAYER_BOX_COUNT; j++)
                //{
                //    Box box = Instantiate(defaultBoxPrefab, layer.transform).GetComponent<Box>();
                //    box.transform.localPosition = new Vector3(j * BOX_GAP, 0f, 0f);
                //    box.Init(this);
                //}
            }
        }
    }
}
