using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public static class CharacterTracker
    {
        #region PLAYER
        public static Transform PlayerTransform { get; private set; }
        public static void SetPlayerTransform(Transform transform) => PlayerTransform = transform;
        #endregion

        #region AI
        private static List<Ai> aisInScene;
        public static List<Ai> AIsInScene => aisInScene == null ? aisInScene = new List<Ai>() : aisInScene;

        public static void AddAi(Ai ai)
        {
            if (!AIsInScene.Contains(ai))
                AIsInScene.Add(ai);
        }

        public static void RemoveAi(Ai ai)
        {
            if (AIsInScene.Contains(ai))
                AIsInScene.Remove(ai);
        }
        #endregion
    }
}
