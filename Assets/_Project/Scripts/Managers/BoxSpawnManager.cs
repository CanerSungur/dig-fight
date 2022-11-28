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

        private const int MAX_EXPLOSIVE_COUNT = 1;
        private int _currentExplosiveCountOnPlayerSide, _currentExplosiveCountOnAiSide = 0;
        #endregion

        #region PROPERTIES
        public Dictionary<Enums.PrefabStamp, GameObject> PrefabDictionary => _prefabDictionary;
        public Transform BoxContainerTransform => boxContainerTransform;
        public bool HasExplosiveOnPlayerSide { get; private set; }
        public bool HasExplosiveOnAiSide { get; private set; }
        #endregion

        public void Init(GameManager gameManager)
        {
            _prefabData = Resources.LoadAll<PrefabSO>("_PrefabData/");
            InitializePrefabDictionary();

            HasExplosiveOnPlayerSide = HasExplosiveOnAiSide = false;
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
                layer.Init(this, _currentLayerCount, LAYER_BOX_COUNT);

                _currentLayerCount++;
            }
        }

        #region PUBLICS
        public void ExplosiveSpawnedOnPlayerSide()
        {
            _currentExplosiveCountOnPlayerSide++;
            HasExplosiveOnPlayerSide = _currentExplosiveCountOnPlayerSide >= MAX_EXPLOSIVE_COUNT;
        }
        public void ExplosiveSpawnedOnAiSide()
        {
            _currentExplosiveCountOnAiSide++;
            HasExplosiveOnAiSide = _currentExplosiveCountOnAiSide >= MAX_EXPLOSIVE_COUNT;
        }
        #endregion
    }
}
