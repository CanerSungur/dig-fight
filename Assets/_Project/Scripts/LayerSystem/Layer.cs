using System.Reflection;
using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;
using ZestCore.Utility;

namespace DigFight
{
    public class Layer : MonoBehaviour
    {
        private BoxSpawnManager _boxSpawnManager;
        private const float BOX_GAP = 2.25f;
        private const int LAYER_BOX_COUNT = 5;

        private const int BORDER_TOP_OFFSET = 3;
        private const int BORDER_BOTTOM_OFFSET = 1;

        private bool _firstLayer, _lastLayer;
        private int _layerNumber;

        public int LayerNumber => _layerNumber;

        public void Init(BoxSpawnManager boxSpawnManager, int number, int totalLayerCount)
        {
            if (_boxSpawnManager == null)
                _boxSpawnManager = boxSpawnManager;

            _layerNumber = number;
            _firstLayer = number == 0;
            _lastLayer = number == totalLayerCount - 1;

            transform.SetParent(_boxSpawnManager.BoxContainerTransform);
            transform.localPosition = new Vector3(0f, number * -BOX_GAP, 0f);

            SpawnPlayerSideBoxes();
            SpawnAiSideBoxes();
        }

        private Transform GetRandomBreakableBox()
        {
            if (RNG.RollDice(80))
                return Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StoneBox], transform).transform;
            else
            {
                if (RNG.RollDice(70))
                    return Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.CopperBox], transform).transform;
                else
                    return Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.DiamondBox], transform).transform;
            }
        }

        #region BOX SPAWN FUNCTIONS
        private void SpawnPlayerSideBoxes()
        {
            for (int j = 0; j < LAYER_BOX_COUNT; j++)
            {
                Transform boxTransform = null;

                if (_firstLayer && j == 0)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                else if (_lastLayer)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
                else
                {
                    boxTransform = GetRandomBreakableBox();
                    boxTransform.GetComponent<BreakableBox>().Init(this);
                }

                boxTransform.localPosition = new Vector3(j * BOX_GAP, 0f, 0f);

                CheckForTopBorderSpawn(j);
                CheckForBottomBorderSpawn(j);
                CheckForLeftBorderSpawn(j);
            }
        }
        private void SpawnAiSideBoxes()
        {
            SpawnMiddleBox();

            for (int j = 0; j < LAYER_BOX_COUNT; j++)
            {
                Transform boxTransform = null;

                if (_firstLayer && j == LAYER_BOX_COUNT - 1)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
                else if (_lastLayer)
                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
                else
                {
                    boxTransform = GetRandomBreakableBox();
                    boxTransform.GetComponent<BreakableBox>().Init(this);
                }

                boxTransform.localPosition = new Vector3(((j + 1) + LAYER_BOX_COUNT) * BOX_GAP, 0f, 0f);

                CheckForTopBorderSpawn((j + 1) + LAYER_BOX_COUNT);
                CheckForBottomBorderSpawn((j + 1) + LAYER_BOX_COUNT);
                CheckForRightBorderSpawn(j);
            }
        }
        private void SpawnMiddleBox()
        {
            GameObject box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
            box.transform.localPosition = new Vector3(BOX_GAP * LAYER_BOX_COUNT, 0f, 0f);

            if (_firstLayer)
            {
                for (int i = 1; i < BORDER_TOP_OFFSET; i++)
                {
                    GameObject gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
                    gapBox.transform.localPosition = new Vector3(BOX_GAP * LAYER_BOX_COUNT, i * BOX_GAP, 0f);
                }

                GameObject borderBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform);
                borderBox.transform.localPosition = new Vector3(BOX_GAP * LAYER_BOX_COUNT, BORDER_TOP_OFFSET * BOX_GAP);
            }
            else if (_lastLayer)
            {
                for (int i = 1; i <= BORDER_BOTTOM_OFFSET; i++)
                {
                    GameObject gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform);
                    gapBox.transform.localPosition = new Vector3(BOX_GAP * LAYER_BOX_COUNT, -i * BOX_GAP, 0f);
                }

                GameObject borderBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform);
                borderBox.transform.localPosition = new Vector3(BOX_GAP * LAYER_BOX_COUNT, -BORDER_BOTTOM_OFFSET * BOX_GAP);
            }
        }
        #endregion

        #region BORDER SPAWN FUNCTIONS
        private void CheckForTopBorderSpawn(int index)
        {
            if (_firstLayer)
            {
                Transform box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(index * BOX_GAP, BORDER_TOP_OFFSET * BOX_GAP, 0f);
            }
        }
        private void CheckForBottomBorderSpawn(int index)
        {
            if (_lastLayer)
            {
                Transform box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(index * BOX_GAP, -BORDER_BOTTOM_OFFSET * BOX_GAP, 0f);
            }
        }
        private void CheckForLeftBorderSpawn(int index)
        {
            if (index == 0)
            {
                Transform box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(-1 * BOX_GAP, 0f, 0f);

                if (_firstLayer)
                {
                    for (int i = 1; i <= BORDER_TOP_OFFSET; i++)
                    {
                        Transform gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(-1 * BOX_GAP, i * BOX_GAP, 0f);
                    }
                }
                else if (_lastLayer)
                {
                    for (int i = 1; i <= BORDER_BOTTOM_OFFSET; i++)
                    {
                        Transform gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(-1 * BOX_GAP, -i * BOX_GAP, 0f);
                    }
                }
            }
        }
        private void CheckForRightBorderSpawn(int index)
        {
            if (index == LAYER_BOX_COUNT - 1)
            {
                Transform box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(((index + 2) + LAYER_BOX_COUNT) * BOX_GAP, 0f, 0f);

                if (_firstLayer)
                {
                    for (int i = 1; i <= BORDER_TOP_OFFSET; i++)
                    {
                        Transform gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(((index + 2) + LAYER_BOX_COUNT) * BOX_GAP, i * BOX_GAP, 0f);
                    }
                }
                else if (_lastLayer)
                {
                    for (int i = 1; i <= BORDER_BOTTOM_OFFSET; i++)
                    {
                        Transform gapBox = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(((index + 2) + LAYER_BOX_COUNT) * BOX_GAP, -i * BOX_GAP, 0f);
                    }
                }
            }
        }
        #endregion
    }
}
