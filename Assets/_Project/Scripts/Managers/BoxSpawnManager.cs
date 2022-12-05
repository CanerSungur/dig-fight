using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;
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
        [SerializeField] private Transform borderBoxContainerTransform;

        #region SPAWN DATA
        private const float BOX_GAP = 2.25f;
        private int _currentLayerCount;

        private const int MAX_EXPLOSIVE_COUNT = 1;
        private int _currentExplosiveCountOnPlayerSide, _currentExplosiveCountOnAiSide = 0;

        private const int MAX_PUSHABLE_LAYER_COUNT = 1;
        private const int PUSHABLE_LAYER_SPAWN_CHANCE_DECREASE = 30;
        private int _currentPushableLayerCount = 0;
        private bool _setLayerForPushable = false;
        #endregion

        #region PROPERTIES
        public Dictionary<Enums.PrefabStamp, GameObject> PrefabDictionary => _prefabDictionary;
        public Transform BoxContainerTransform => boxContainerTransform;
        public Transform BorderBoxContainerTransform => borderBoxContainerTransform;
        public bool HasExplosiveOnPlayerSide { get; private set; }
        public bool HasExplosiveOnAiSide { get; private set; }
        #endregion

        public void Init(GameManager gameManager)
        {
            _prefabData = Resources.LoadAll<PrefabSO>("_PrefabData/");
            InitializePrefabDictionary();

            HasExplosiveOnPlayerSide = HasExplosiveOnAiSide = false;
            _currentLayerCount = _currentPushableLayerCount = 0;

            SpawnLayers();
        }

        #region HELPERS
        private void InitializePrefabDictionary()
        {
            _prefabDictionary = new Dictionary<Enums.PrefabStamp, GameObject>();

            for (int i = 0; i < _prefabData.Length; i++)
            {
                if (!_prefabDictionary.ContainsKey(_prefabData[i].Stamp))
                    _prefabDictionary.Add(_prefabData[i].Stamp, _prefabData[i].Prefab);
            }
        }
        private void DecidePushableOrBreakableLayer()
        {
            _setLayerForPushable = _currentLayerCount > 2 && _currentLayerCount < layerCount - 1 && _currentPushableLayerCount < MAX_PUSHABLE_LAYER_COUNT && RNG.RollDice(100 - (_currentPushableLayerCount * PUSHABLE_LAYER_SPAWN_CHANCE_DECREASE));
            if (_setLayerForPushable) _currentPushableLayerCount++;
        }
        private void CombineBorderBoxMeshes()
        {
            MeshCombiner meshCombiner = borderBoxContainerTransform.gameObject.AddComponent<MeshCombiner>();
            meshCombiner.CreateMultiMaterialMesh = true;
            meshCombiner.DestroyCombinedChildren = true;
            meshCombiner.CombineMeshes(true);
        }
        #endregion

        private void SpawnLayers()
        {
            for (int i = 0; i < layerCount; i++)
            {
                DecidePushableOrBreakableLayer();

                Layer layer = new GameObject($"Layer_{_currentLayerCount}", typeof(Layer), typeof(LayerBorderBoxHandler), typeof(LayerBreakableBoxHandler), typeof(LayerExplosiveBoxHandler), typeof(LayerPushableBoxHandler)).GetComponent<Layer>();
                layer.Init(this, _currentLayerCount, layerCount, _setLayerForPushable);

                _currentLayerCount++;

                if (_setLayerForPushable == true) _setLayerForPushable = false;
            }

            CombineBorderBoxMeshes();
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
