using System.Collections.Generic;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace DigFight
{
    public class BoxSpawnManager : MonoBehaviour
    {
        //#region PREFAB DATA SECTION
        //private PrefabSO[] _prefabData;
        //private Dictionary<Enums.PrefabStamp, GameObject> _prefabDictionary;
        //#endregion

        //[Header("-- SETUP --")]
        //[SerializeField] private int layerCount = 5;
        //[SerializeField] private Transform boxContainerTransform;
        //[SerializeField] private Transform borderBoxContainerTransform;

        //#region SPAWN DATA
        //private Enums.LayerType _currentLayerType;

        //private const float BOX_GAP = 2.25f;
        //private int _currentLayerCount;

        //private const int MAX_EXPLOSIVE_COUNT = 1;
        //private int _currentExplosiveCountOnPlayerSide, _currentExplosiveCountOnAiSide = 0;

        //private const int MAX_PUSHABLE_LAYER_COUNT = 1;
        //private const int PUSHABLE_LAYER_SPAWN_CHANCE_DECREASE = 30;
        //private int _currentPushableLayerCount = 0;
        ////private bool _setLayerForPushable = false;

        //private const int MAX_PICKAXE_DURABILITY_CHEST_COUNT = 1;
        //private const int MAX_PICKAXE_SPEED_CHEST_COUNT = 1;
        //private const int CHEST_SPAWN_CHANCE_INCREMENT = 15;
        //private int _currentPickaxeDurabilityChestCount, _currentPickaxeSpeedChestCount = 0;
        //#endregion

        //#region PROPERTIES
        //public Dictionary<Enums.PrefabStamp, GameObject> PrefabDictionary => _prefabDictionary;
        //public Transform BoxContainerTransform => boxContainerTransform;
        //public Transform BorderBoxContainerTransform => borderBoxContainerTransform;
        //public bool HasExplosiveOnPlayerSide { get; private set; }
        //public bool HasExplosiveOnAiSide { get; private set; }
        //#endregion

        //#region STATICS
        //public static int LayerCount { get; private set; }
        //#endregion

        //public void Init(GameManager gameManager)
        //{
        //    _prefabData = Resources.LoadAll<PrefabSO>("_PrefabData/");
        //    InitializePrefabDictionary();

        //    HasExplosiveOnPlayerSide = HasExplosiveOnAiSide = false;
        //    _currentLayerCount = _currentPushableLayerCount = 0;
        //    LayerCount = layerCount;

        //    SpawnLayers();
        //}

        //#region HELPERS
        //private void InitializePrefabDictionary()
        //{
        //    _prefabDictionary = new Dictionary<Enums.PrefabStamp, GameObject>();

        //    for (int i = 0; i < _prefabData.Length; i++)
        //    {
        //        if (!_prefabDictionary.ContainsKey(_prefabData[i].Stamp))
        //            _prefabDictionary.Add(_prefabData[i].Stamp, _prefabData[i].Prefab);
        //    }
        //}
        //private bool DecidePushableOrBreakableLayer()
        //{
        //    if (_currentLayerCount > 2 && _currentLayerCount < layerCount - 1 && _currentPushableLayerCount < MAX_PUSHABLE_LAYER_COUNT && RNG.RollDice(100 - (_currentPushableLayerCount * PUSHABLE_LAYER_SPAWN_CHANCE_DECREASE)))
        //    {
        //        _currentPushableLayerCount++;
        //        _currentLayerType = Enums.LayerType.Pushable;
        //        return true;
        //    }
        //    else return false;
        //}
        //private bool DecideDurabilityChestLayer()
        //{
        //    if (_currentLayerCount > 1 && _currentLayerCount < layerCount - 1 && _currentPickaxeDurabilityChestCount < MAX_PICKAXE_DURABILITY_CHEST_COUNT && RNG.RollDice(20 + (_currentLayerCount * CHEST_SPAWN_CHANCE_INCREMENT)))
        //    {
        //        _currentPickaxeDurabilityChestCount++;
        //        _currentLayerType = Enums.LayerType.DurabilityChest;
        //        return true;
        //    }
        //    else return false;
        //}
        //private bool DecideSpeedChestLayer()
        //{
        //    if (_currentLayerCount > 1 && _currentLayerCount < layerCount - 1 && _currentPickaxeSpeedChestCount < MAX_PICKAXE_SPEED_CHEST_COUNT && RNG.RollDice(20 + (_currentLayerCount * CHEST_SPAWN_CHANCE_INCREMENT)))
        //    {
        //        _currentPickaxeSpeedChestCount++;
        //        _currentLayerType = Enums.LayerType.SpeedChest;
        //        return true;
        //    }
        //    else return false;
        //}
        //private void CombineBorderBoxMeshes()
        //{
        //    MeshCombiner meshCombiner = borderBoxContainerTransform.gameObject.AddComponent<MeshCombiner>();
        //    meshCombiner.CreateMultiMaterialMesh = true;
        //    meshCombiner.DestroyCombinedChildren = true;
        //    meshCombiner.CombineMeshes(false);
        //}
        //#endregion

        //private void SpawnLayers()
        //{
        //    for (int i = 0; i < layerCount; i++)
        //    {
        //        if (!DecidePushableOrBreakableLayer())
        //            if (!DecideDurabilityChestLayer())
        //                if (!DecideSpeedChestLayer())
        //                    _currentLayerType = Enums.LayerType.Breakable;

        //        Layer layer = new GameObject($"Layer_{_currentLayerCount}", typeof(Layer), typeof(LayerBorderBoxHandler), typeof(LayerBreakableBoxHandler), typeof(LayerExplosiveBoxHandler), typeof(LayerPushableBoxHandler)).GetComponent<Layer>();
        //        layer.Init(this, _currentLayerCount, layerCount, _currentLayerType);

        //        _currentLayerCount++;

        //        //if (_setLayerForPushable == true) _setLayerForPushable = false;
        //    }

        //    CombineBorderBoxMeshes();
        //}

        //#region PUBLICS
        //public void ExplosiveSpawnedOnPlayerSide()
        //{
        //    _currentExplosiveCountOnPlayerSide++;
        //    HasExplosiveOnPlayerSide = _currentExplosiveCountOnPlayerSide >= MAX_EXPLOSIVE_COUNT;
        //}
        //public void ExplosiveSpawnedOnAiSide()
        //{
        //    _currentExplosiveCountOnAiSide++;
        //    HasExplosiveOnAiSide = _currentExplosiveCountOnAiSide >= MAX_EXPLOSIVE_COUNT;
        //}
        //#endregion
    }
}
