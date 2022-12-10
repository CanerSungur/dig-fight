using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ZestCore.Utility
{
    public static class Utils
    {
#if UNITY_EDITOR
        public static List<T> FindAllScriptableObjectsOfType<T>(string filter, string folder = "Assets")
            where T : ScriptableObject
        {
            return AssetDatabase.FindAssets(filter, new[] { folder })
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
        }
#endif
    }
}
