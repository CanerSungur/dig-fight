using UnityEngine;

namespace ZestGames
{
    public class QueueManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private ExampleQueue exampleQueue;

        #region STATIC QUEUES
        public static ExampleQueue ExampleQueue { get; private set; }
        #endregion
        
        public void Init(GameManager gameManager)
        {
            ExampleQueue = exampleQueue;
        }
    }
}
