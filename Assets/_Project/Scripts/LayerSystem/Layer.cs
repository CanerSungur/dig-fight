using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;
using ZestCore.Utility;

namespace DigFight
{
    public class Layer : MonoBehaviour
    {
        private BoxSpawnManager _boxSpawnManager;

        #region SCRIPT REFERENCES
        private LayerBorderBoxHandler _borderBoxHandler;
        public LayerBorderBoxHandler BorderBoxHandler => _borderBoxHandler == null ? _borderBoxHandler = GetComponent<LayerBorderBoxHandler>() : _borderBoxHandler;
        private LayerBreakableBoxHandler _breakableBoxHandler;
        public LayerBreakableBoxHandler BreakableBoxHandler => _breakableBoxHandler == null ? _breakableBoxHandler = GetComponent<LayerBreakableBoxHandler>() : _breakableBoxHandler;
        private LayerExplosiveBoxHandler _explosiveBoxHandler;
        public LayerExplosiveBoxHandler ExplosiveBoxHandler => _explosiveBoxHandler == null ? _explosiveBoxHandler = GetComponent<LayerExplosiveBoxHandler>() : _explosiveBoxHandler;
        private LayerPushableBoxHandler _pushableBoxHandler;
        public LayerPushableBoxHandler PushableBoxHandler => _pushableBoxHandler == null ? _pushableBoxHandler = GetComponent<LayerPushableBoxHandler>() : _pushableBoxHandler;
        #endregion

        #region SPAWN DATA
        private Enums.LayerType _layerType;
        private int _layerNumber;
        public readonly float BoxGap = 2.25f;
        public readonly int BoxCount = 5;
        private int _randomChestSpawnIndex;
        #endregion

        #region PROPERTIES
        public BoxSpawnManager BoxSpawnManager => _boxSpawnManager;
        public int LayerNumber => _layerNumber;
        public bool PlayerSideCanHasExplosive => !FirstLayer && !LastLayer && !ExplosiveBoxHandler.HasExplosiveOnPlayerSide && !_boxSpawnManager.HasExplosiveOnPlayerSide && RNG.RollDice(30 * _layerNumber);
        public bool AiSideCanHasExplosive => !FirstLayer && !LastLayer && !ExplosiveBoxHandler.HasExplosiveOnAiSide && !_boxSpawnManager.HasExplosiveOnAiSide && RNG.RollDice(30 * _layerNumber);
        public bool FirstLayer { get; private set; }
        public bool LastLayer { get; private set; }
        #endregion

        public void Init(BoxSpawnManager boxSpawnManager, int number, int totalLayerCount, Enums.LayerType layerType)
        {
            if (_boxSpawnManager == null)
                _boxSpawnManager = boxSpawnManager;

            _layerType = layerType;
            _layerNumber = number;
            FirstLayer = number == 0;
            LastLayer = number == totalLayerCount - 1;

            transform.SetParent(_boxSpawnManager.BoxContainerTransform);
            transform.localPosition = new Vector3(0f, number * -BoxGap, 0f);

            BorderBoxHandler.Init(this);
            BreakableBoxHandler.Init(this);
            ExplosiveBoxHandler.Init(this);
            PushableBoxHandler.Init(this);

            DecideLayerTypeSpawn();
            SpawnMiddleBoxes(BorderBoxHandler.TopOffset, BorderBoxHandler.BottomOffset);
        }

        #region HELPERS
        private void SetParentAsBorderBoxContainer(Transform borderBoxTransform) => borderBoxTransform.SetParent(_boxSpawnManager.BorderBoxContainerTransform);
        private void DecideLayerTypeSpawn()
        {
            if (_layerType == Enums.LayerType.Pushable)
            {
                // spawn pushable layer
                PushableBoxHandler.SpawnPushableBoxesForPlayerSide(this);
                PushableBoxHandler.SpawnPushableBoxesForAiSide(this);
            }
            else if (_layerType == Enums.LayerType.DurabilityChest || _layerType == Enums.LayerType.SpeedChest)
            {
                _randomChestSpawnIndex = Random.Range(1, BoxCount - 1); // We exclude first and last index
                SpawnPlayerSideBoxes();
                SpawnAiSideBoxes();
            }
            else
            {
                SpawnPlayerSideBoxes();
                SpawnAiSideBoxes();
            }
        }
        #endregion

        #region SPAWN FUNCTIONS
        private void SpawnPlayerSideBoxes()
        {
            for (int j = 0; j < BoxCount; j++)
            {
                Transform boxTransform = null;

                if (FirstLayer && j == 0)
                {
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                    boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
                    SetParentAsBorderBoxContainer(boxTransform);
                }
                else if (LastLayer)
                {
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
                    boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
                    SetParentAsBorderBoxContainer(boxTransform);
                }
                else
                {
                    if (PlayerSideCanHasExplosive)
                        ExplosiveBoxHandler.SpawnExplosiveBoxForPlayerSide(out boxTransform);
                    else
                    {
                        if (j == _randomChestSpawnIndex && _layerType == Enums.LayerType.DurabilityChest)
                            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ChestPickaxeDurability], transform).transform;
                        else if (j == _randomChestSpawnIndex && _layerType == Enums.LayerType.SpeedChest)
                            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ChestPickaxeSpeed], transform).transform;
                        else
                            boxTransform = BreakableBoxHandler.GetRandomBreakableBox();
                    }

                    if (boxTransform.TryGetComponent(out BreakableBox breakableBox))
                        breakableBox.Init(this);
                    else if (boxTransform.TryGetComponent(out ExplosiveBox explosiveBox))
                        explosiveBox.Init(this);

                    boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
                }

                BorderBoxHandler.SpawnTopBorder(j);
                BorderBoxHandler.SpawnBottomBorder(j);
                BorderBoxHandler.SpawnLeftBorder(j);
            }
        }
        private void SpawnAiSideBoxes()
        {
            for (int j = 0; j < BoxCount; j++)
            {
                Transform boxTransform = null;

                if (FirstLayer && j == BoxCount - 1)
                {
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                    boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
                    SetParentAsBorderBoxContainer(boxTransform);
                }
                else if (LastLayer)
                {
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
                    boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
                    SetParentAsBorderBoxContainer(boxTransform);
                }
                else
                {
                    if (AiSideCanHasExplosive)
                        ExplosiveBoxHandler.SpawnExplosiveBoxForAiSide(out boxTransform);
                    else
                        boxTransform = BreakableBoxHandler.GetRandomBreakableBox();

                    if (boxTransform.TryGetComponent(out BreakableBox breakableBox))
                        breakableBox.Init(this);
                    else if (boxTransform.TryGetComponent(out ExplosiveBox explosiveBox))
                        explosiveBox.Init(this);

                    boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
                }

                BorderBoxHandler.SpawnTopBorder((j + 1) + BoxCount);
                BorderBoxHandler.SpawnBottomBorder((j + 1) + BoxCount);
                BorderBoxHandler.SpawnRightBorder(j);
            }
        }
        private void SpawnMiddleBoxes(float borderTopOffset, float borderBottomOffset)
        {
            Transform boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
            boxTransform.localPosition = new Vector3(BoxGap * BoxCount, 0f, 0f);
            SetParentAsBorderBoxContainer(boxTransform);

            if (FirstLayer)
            {
                for (int i = 1; i < borderTopOffset; i++)
                {
                    Transform gapBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
                    gapBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, i * BoxGap, 0f);
                    SetParentAsBorderBoxContainer(gapBoxTransform);
                }

                Transform borderBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                borderBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, borderTopOffset * BoxGap);
                SetParentAsBorderBoxContainer(borderBoxTransform);
            }
            else if (LastLayer)
            {
                for (int i = 1; i <= borderBottomOffset; i++)
                {
                    Transform gapBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
                    gapBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, -i * BoxGap, 0f);
                    SetParentAsBorderBoxContainer(gapBoxTransform);
                }

                Transform borderBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                borderBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, -borderBottomOffset * BoxGap);
                SetParentAsBorderBoxContainer(borderBoxTransform);
            }
        }
        #endregion
    }
}
