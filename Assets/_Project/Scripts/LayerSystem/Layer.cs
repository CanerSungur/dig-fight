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
        private int _layerNumber;
        public readonly float BoxGap = 2.25f;
        public readonly int BoxCount = 5;
        #endregion

        #region PROPERTIES
        public BoxSpawnManager BoxSpawnManager => _boxSpawnManager;
        public int LayerNumber => _layerNumber;
        public bool PlayerSideCanHasExplosive => !FirstLayer && !LastLayer && !ExplosiveBoxHandler.HasExplosiveOnPlayerSide && !_boxSpawnManager.HasExplosiveOnPlayerSide && RNG.RollDice(30 * _layerNumber);
        public bool AiSideCanHasExplosive => !FirstLayer && !LastLayer && !ExplosiveBoxHandler.HasExplosiveOnAiSide && !_boxSpawnManager.HasExplosiveOnAiSide && RNG.RollDice(30 * _layerNumber);
        public bool FirstLayer { get; private set; }
        public bool LastLayer { get; private set; }
        #endregion

        public void Init(BoxSpawnManager boxSpawnManager, int number, int totalLayerCount, bool setLayerForPushable)
        {
            if (_boxSpawnManager == null)
                _boxSpawnManager = boxSpawnManager;

            _layerNumber = number;
            FirstLayer = number == 0;
            LastLayer = number == totalLayerCount - 1;

            transform.SetParent(_boxSpawnManager.BoxContainerTransform);
            transform.localPosition = new Vector3(0f, number * -BoxGap, 0f);

            BorderBoxHandler.Init(this);
            BreakableBoxHandler.Init(this);
            ExplosiveBoxHandler.Init(this);
            PushableBoxHandler.Init(this);

            if (setLayerForPushable)
            {
                // spawn pushable layer
                PushableBoxHandler.SpawnPushableBoxesForPlayerSide(this);
                PushableBoxHandler.SpawnPushableBoxesForAiSide(this);
            }
            else
            {
                SpawnPlayerSideBoxes();
                SpawnAiSideBoxes();
            }
            
            SpawnMiddleBoxes(BorderBoxHandler.TopOffset, BorderBoxHandler.BottomOffset);
        }

        private void SpawnPlayerSideBoxes()
        {
            for (int j = 0; j < BoxCount; j++)
            {
                Transform boxTransform = null;

                if (FirstLayer && j == 0)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                else if (LastLayer)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
                else
                {
                    if (PlayerSideCanHasExplosive)
                        ExplosiveBoxHandler.SpawnExplosiveBoxForPlayerSide(out boxTransform);
                    else
                        boxTransform = BreakableBoxHandler.GetRandomBreakableBox();

                    if (boxTransform.TryGetComponent(out BreakableBox breakableBox))
                        breakableBox.Init(this);
                    else if (boxTransform.TryGetComponent(out ExplosiveBox explosiveBox))
                        explosiveBox.Init(this);
                }

                boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);

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
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                else if (LastLayer)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
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
                }
                boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);

                BorderBoxHandler.SpawnTopBorder((j + 1) + BoxCount);
                BorderBoxHandler.SpawnBottomBorder((j + 1) + BoxCount);
                BorderBoxHandler.SpawnRightBorder(j);
            }
        }
        private void SpawnMiddleBoxes(float borderTopOffset, float borderBottomOffset)
        {
            GameObject box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
            box.transform.localPosition = new Vector3(BoxGap * BoxCount, 0f, 0f);

            if (FirstLayer)
            {
                for (int i = 1; i < borderTopOffset; i++)
                {
                    GameObject gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
                    gapBox.transform.localPosition = new Vector3(BoxGap * BoxCount, i * BoxGap, 0f);
                }

                GameObject borderBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform);
                borderBox.transform.localPosition = new Vector3(BoxGap * BoxCount, borderTopOffset * BoxGap);
            }
            else if (LastLayer)
            {
                for (int i = 1; i <= borderBottomOffset; i++)
                {
                    GameObject gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
                    gapBox.transform.localPosition = new Vector3(BoxGap * BoxCount, -i * BoxGap, 0f);
                }

                GameObject borderBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform);
                borderBox.transform.localPosition = new Vector3(BoxGap * BoxCount, -borderBottomOffset * BoxGap);
            }
        }
    }
}
