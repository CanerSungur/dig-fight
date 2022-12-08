using UnityEngine;
using ZestGames;
using Random = UnityEngine.Random;
using ZestCore.Utility;
using System.Collections.Generic;

namespace DigFight
{
    public class Layer : MonoBehaviour
    {
        #region COMPONENTS
        private LayerHandler _layerHandler;
        private GameObject _borderContainer, _boxContainer;
        #endregion

        #region LIST & ARRAYS
        private IBoxInteractable[] _interactableBoxes;
        private List<PushableBox> _pushableBoxes = new List<PushableBox>();
        #endregion

        public readonly int BoxCount = 4;
        public readonly float BoxGap = 2.25f;

        public void Init(LayerHandler layerHandler)
        {
            _layerHandler = layerHandler;
            _borderContainer = transform.GetChild(0).gameObject;
            _boxContainer = transform.GetChild(1).gameObject;
            InitializeInteractiveBoxes();
        }

        #region HELPERS
        private void InitializeInteractiveBoxes()
        {
            _interactableBoxes = GetComponentsInChildren<IBoxInteractable>();
            for (int i = 0; i < _interactableBoxes.Length; i++)
            {
                _interactableBoxes[i].Init(this);
                _interactableBoxes[i].ChangeParent(transform);
            }

            Destroy(_boxContainer);
            _borderContainer.transform.SetParent(_layerHandler.BorderContainer);
        }
        
        #endregion

        #region PUBLICS
        public void PushBoxesLeft()
        {
            foreach (PushableBox pushableBox in _pushableBoxes)
            {
                pushableBox.StartMoveSequence(Enums.BoxTriggerDirection.Left);
            }
        }
        public void PushBoxesRight()
        {
            foreach (PushableBox pushableBox in _pushableBoxes)
            {
                pushableBox.StartMoveSequence(Enums.BoxTriggerDirection.Right);
            }
        }
        public void AddPushableBox(PushableBox pushableBox)
        {
            if (!_pushableBoxes.Contains(pushableBox))
                _pushableBoxes.Add(pushableBox);
        }
        public void RemovePushableBox(PushableBox pushableBox)
        {
            if (_pushableBoxes.Contains(pushableBox))
                _pushableBoxes.Remove(pushableBox);
        }
        #endregion

        //private BoxSpawnManager _boxSpawnManager;

        //#region SCRIPT REFERENCES
        //private LayerBorderBoxHandler _borderBoxHandler;
        //public LayerBorderBoxHandler BorderBoxHandler => _borderBoxHandler == null ? _borderBoxHandler = GetComponent<LayerBorderBoxHandler>() : _borderBoxHandler;
        //private LayerBreakableBoxHandler _breakableBoxHandler;
        //public LayerBreakableBoxHandler BreakableBoxHandler => _breakableBoxHandler == null ? _breakableBoxHandler = GetComponent<LayerBreakableBoxHandler>() : _breakableBoxHandler;
        //private LayerExplosiveBoxHandler _explosiveBoxHandler;
        //public LayerExplosiveBoxHandler ExplosiveBoxHandler => _explosiveBoxHandler == null ? _explosiveBoxHandler = GetComponent<LayerExplosiveBoxHandler>() : _explosiveBoxHandler;
        //private LayerPushableBoxHandler _pushableBoxHandler;
        //public LayerPushableBoxHandler PushableBoxHandler => _pushableBoxHandler == null ? _pushableBoxHandler = GetComponent<LayerPushableBoxHandler>() : _pushableBoxHandler;
        //#endregion

        //#region SPAWN FUNCTIONS
        //private void SpawnPlayerSideBoxes()
        //{
        //    for (int j = 0; j < BoxCount; j++)
        //    {
        //        Transform boxTransform = null;

        //        if (FirstLayer && j == 0)
        //        {
        //            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
        //            boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
        //            SetParentAsBorderBoxContainer(boxTransform);
        //        }
        //        else if (LastLayer)
        //        {
        //            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
        //            boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
        //            SetParentAsBorderBoxContainer(boxTransform);
        //        }
        //        else
        //        {
        //            if (PlayerSideCanHasExplosive)
        //                ExplosiveBoxHandler.SpawnExplosiveBoxForPlayerSide(out boxTransform);
        //            else
        //            {
        //                if (j == _randomChestSpawnIndex && _layerType == Enums.LayerType.DurabilityChest)
        //                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ChestPickaxeDurability], transform).transform;
        //                else if (j == _randomChestSpawnIndex && _layerType == Enums.LayerType.SpeedChest)
        //                    boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ChestPickaxeSpeed], transform).transform;
        //                else
        //                    boxTransform = BreakableBoxHandler.GetRandomBreakableBox();
        //            }

        //            if (boxTransform.TryGetComponent(out BreakableBox breakableBox))
        //                breakableBox.Init(this);
        //            else if (boxTransform.TryGetComponent(out ExplosiveBox explosiveBox))
        //                explosiveBox.Init(this);

        //            boxTransform.localPosition = new Vector3(j * BoxGap, 0f, 0f);
        //        }

        //        BorderBoxHandler.SpawnTopBorder(j);
        //        BorderBoxHandler.SpawnBottomBorder(j);
        //        BorderBoxHandler.SpawnLeftBorder(j);
        //    }
        //}
        //private void SpawnAiSideBoxes()
        //{
        //    for (int j = 0; j < BoxCount; j++)
        //    {
        //        Transform boxTransform = null;

        //        if (FirstLayer && j == BoxCount - 1)
        //        {
        //            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.StaticBox], transform).transform;
        //            boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
        //            SetParentAsBorderBoxContainer(boxTransform);
        //        }
        //        else if (LastLayer)
        //        {
        //            boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.LevelEndBox], transform).transform;
        //            boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
        //            SetParentAsBorderBoxContainer(boxTransform);
        //        }
        //        else
        //        {
        //            if (AiSideCanHasExplosive)
        //                ExplosiveBoxHandler.SpawnExplosiveBoxForAiSide(out boxTransform);
        //            else
        //                boxTransform = BreakableBoxHandler.GetRandomBreakableBox();

        //            if (boxTransform.TryGetComponent(out BreakableBox breakableBox))
        //                breakableBox.Init(this);
        //            else if (boxTransform.TryGetComponent(out ExplosiveBox explosiveBox))
        //                explosiveBox.Init(this);

        //            boxTransform.localPosition = new Vector3(((j + 1) + BoxCount) * BoxGap, 0f, 0f);
        //        }

        //        BorderBoxHandler.SpawnTopBorder((j + 1) + BoxCount);
        //        BorderBoxHandler.SpawnBottomBorder((j + 1) + BoxCount);
        //        BorderBoxHandler.SpawnRightBorder(j);
        //    }
        //}
        //private void SpawnMiddleBoxes(float borderTopOffset, float borderBottomOffset)
        //{
        //    Transform boxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
        //    boxTransform.localPosition = new Vector3(BoxGap * BoxCount, 0f, 0f);
        //    SetParentAsBorderBoxContainer(boxTransform);

        //    if (FirstLayer)
        //    {
        //        for (int i = 1; i < borderTopOffset; i++)
        //        {
        //            Transform gapBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
        //            gapBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, i * BoxGap, 0f);
        //            SetParentAsBorderBoxContainer(gapBoxTransform);
        //        }

        //        Transform borderBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
        //        borderBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, borderTopOffset * BoxGap);
        //        SetParentAsBorderBoxContainer(borderBoxTransform);
        //    }
        //    else if (LastLayer)
        //    {
        //        for (int i = 1; i <= borderBottomOffset; i++)
        //        {
        //            Transform gapBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.MiddleBox], transform).transform;
        //            gapBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, -i * BoxGap, 0f);
        //            SetParentAsBorderBoxContainer(gapBoxTransform);
        //        }

        //        Transform borderBoxTransform = Instantiate(_boxSpawnManager.PrefabDictionary[Enums.PrefabStamp.BorderBox], transform).transform;
        //        borderBoxTransform.transform.localPosition = new Vector3(BoxGap * BoxCount, -borderBottomOffset * BoxGap);
        //        SetParentAsBorderBoxContainer(borderBoxTransform);
        //    }
        //}
        //#endregion
    }
}
