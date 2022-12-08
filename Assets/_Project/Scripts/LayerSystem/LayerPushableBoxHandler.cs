using System.Collections.Generic;
using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;

namespace DigFight
{
    public class LayerPushableBoxHandler : MonoBehaviour
    {
        private Layer _layer;

        private List<PushableBox> _pushableBoxes;
        public List<PushableBox> PushableBoxes => _pushableBoxes == null ? _pushableBoxes = new List<PushableBox>() : _pushableBoxes;

        public void Init(Layer layer)
        {
            if (_layer == null)
                _layer = layer;
        }

        #region SPAWN FUNCTIONS
        public void SpawnPushableBoxesForPlayerSide(Layer layer)
        {
            int randomGapIndex = Random.Range(1, layer.BoxCount);

            for (int j = 0; j < layer.BoxCount; j++)
            {
                if (j != randomGapIndex)
                {
                    PushableBox pushableBox = null;

                    pushableBox = Instantiate(layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.PushableBox], transform).GetComponent<PushableBox>();
                    pushableBox.GetComponent<PushableBox>().Init(layer);
                    pushableBox.transform.localPosition = new Vector3(j * layer.BoxGap, 0f, 0f);
                    AddPushableBox(pushableBox);

                    layer.BorderBoxHandler.SpawnTopBorder(j);
                    layer.BorderBoxHandler.SpawnBottomBorder(j);
                    layer.BorderBoxHandler.SpawnLeftBorder(j);
                }
            }
        }
        public void SpawnPushableBoxesForAiSide(Layer layer)
        {
            int randomGapIndex = Random.Range(0, layer.BoxCount - 1);

            for (int j = 0; j < layer.BoxCount; j++)
            {
                if (j != randomGapIndex)
                {
                    PushableBox pushableBox = null;

                    pushableBox = Instantiate(layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.PushableBox], transform).GetComponent<PushableBox>();
                    pushableBox.Init(layer);
                    pushableBox.transform.localPosition = new Vector3(((j + 1) + layer.BoxCount) * layer.BoxGap, 0f, 0f);
                    AddPushableBox(pushableBox);

                    layer.BorderBoxHandler.SpawnTopBorder((j + 1) + layer.BoxCount);
                    layer.BorderBoxHandler.SpawnBottomBorder((j + 1) + layer.BoxCount);
                    layer.BorderBoxHandler.SpawnRightBorder(j);
                }
            }
        }
        #endregion

        #region PUBLICS
        public void PushBoxesLeft()
        {
            foreach (PushableBox pushableBox in PushableBoxes)
            {
                pushableBox.StartMoveSequence(Enums.BoxTriggerDirection.Left);
            }
        }
        public void PushBoxesRight()
        {
            foreach (PushableBox pushableBox in PushableBoxes)
            {
                pushableBox.StartMoveSequence(Enums.BoxTriggerDirection.Right);
            }
        }
        #endregion

        #region HELPERS
        private void AddPushableBox(PushableBox pushableBox)
        {
            if (!PushableBoxes.Contains(pushableBox))
                PushableBoxes.Add(pushableBox);
        }
        private void RemovePushableBox(PushableBox pushableBox)
        {
            if (PushableBoxes.Contains(pushableBox))
                PushableBoxes.Remove(pushableBox);
        }
        #endregion
    }
}
