using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class LayerExplosiveBoxHandler : MonoBehaviour
    {
        //private Layer _layer;

        //public bool HasExplosiveOnPlayerSide { get; private set; }
        //public bool HasExplosiveOnAiSide { get; private set; }

        //public void Init(Layer layer)
        //{
        //    if (_layer == null)
        //        _layer = layer;

        //    HasExplosiveOnPlayerSide = HasExplosiveOnAiSide = false;
        //}

        //#region PUBLICS
        //public void SpawnExplosiveBoxForPlayerSide(out Transform boxTransform)
        //{
        //    boxTransform = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ExplosiveBox], transform).transform;
        //    HasExplosiveOnPlayerSide = true;
        //    _layer.BoxSpawnManager.ExplosiveSpawnedOnPlayerSide();
        //}
        //public void SpawnExplosiveBoxForAiSide(out Transform boxTransform)
        //{
        //    boxTransform = Instantiate(_layer.BoxSpawnManager.PrefabDictionary[Enums.PrefabStamp.ExplosiveBox], transform).transform;
        //    HasExplosiveOnAiSide = true;
        //    _layer.BoxSpawnManager.ExplosiveSpawnedOnAiSide();
        //}
        //#endregion
    }
}
