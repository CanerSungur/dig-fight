using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class Layer : MonoBehaviour
    {
        private BoxSpawnManager _boxSpawnManager;
        private const float BOX_GAP = 2.25f;
        private const float LAYER_BOX_COUNT = 5;

        private bool _firstLayer;

        public void Init(BoxSpawnManager boxSpawnManager, int number)
        {
            if (_boxSpawnManager == null)
                _boxSpawnManager = boxSpawnManager;

            _firstLayer = number == 0;

            transform.SetParent(_boxSpawnManager.BoxContainerTransform);
            transform.localPosition = new Vector3(0f, number * -BOX_GAP, 0f);

            SpawnBoxes();
        }

        private void SpawnBoxes()
        {
            for (int j = 0; j < LAYER_BOX_COUNT; j++)
            {
                Box box;

                if (_firstLayer && j == 0)
                    box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).GetComponent<Box>();
                else
                    box = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.DefaultBox], transform).GetComponent<Box>();

                box.transform.localPosition = new Vector3(j * BOX_GAP, 0f, 0f);
                box.Init(this);
            }
        }
    }
}
