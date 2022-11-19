using UnityEngine;
using ZestGames;

namespace DigFight
{
    [CreateAssetMenu(fileName = "Assets/Resources/_PrefabData/NewPrefabData", menuName = "Create Prefab Data/New Prefab Object")]
    public class PrefabSO : ScriptableObject
    {
        public Enums.PrefabStamp Stamp;
        public GameObject Prefab;
    }
}
