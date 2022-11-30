using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class LayerBorderBoxHandler : MonoBehaviour
    {
        private Layer _layer;

        public readonly int TopOffset = 3;
        public readonly int BottomOffset = 1;

        public void Init(Layer layer)
        {
            if (_layer == null)
                _layer = layer;
        }

        #region PUBLICS
        public void SpawnTopBorder(int index)
        {
            if (_layer.FirstLayer)
            {
                Transform box = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(index * _layer.BoxGap, TopOffset * _layer.BoxGap, 0f);
            }
        }
        public void SpawnBottomBorder(int index)
        {
            if (_layer.LastLayer)
            {
                Transform box = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(index * _layer.BoxGap, -BottomOffset * _layer.BoxGap, 0f);
            }
        }
        public void SpawnLeftBorder(int index)
        {
            if (index == 0)
            {
                Transform box = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(-1 * _layer.BoxGap, 0f, 0f);

                if (_layer.FirstLayer)
                {
                    for (int i = 1; i <= TopOffset; i++)
                    {
                        Transform gapBox = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(-1 * _layer.BoxGap, i * _layer.BoxGap, 0f);
                    }
                }
                else if (_layer.LastLayer)
                {
                    for (int i = 1; i <= BottomOffset; i++)
                    {
                        Transform gapBox = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(-1 * _layer.BoxGap, -i * _layer.BoxGap, 0f);
                    }
                }
            }
        }
        public void SpawnRightBorder(int index)
        {
            if (index == _layer.BoxCount - 1)
            {
                Transform box = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                box.localPosition = new Vector3(((index + 2) + _layer.BoxCount) * _layer.BoxGap, 0f, 0f);

                if (_layer.FirstLayer)
                {
                    for (int i = 1; i <= TopOffset; i++)
                    {
                        Transform gapBox = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(((index + 2) + _layer.BoxCount) * _layer.BoxGap, i * _layer.BoxGap, 0f);
                    }
                }
                else if (_layer.LastLayer)
                {
                    for (int i = 1; i <= BottomOffset; i++)
                    {
                        Transform gapBox = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
                        gapBox.localPosition = new Vector3(((index + 2) + _layer.BoxCount) * _layer.BoxGap, -i * _layer.BoxGap, 0f);
                    }
                }
            }
        }
        #endregion
    }
}
