using UnityEngine;

namespace ZestGames
{
    public class QueuePoint : MonoBehaviour
    {
        private QueueSystem _queueSystem;

        #region PROPERTIES
        public bool IsEmpty { get; private set; }
        public int Number { get; private set; }
        public QueueSystem QueueSystem => _queueSystem;
        #endregion

        public void Init(QueueSystem queueSystem, int number)
        {
            _queueSystem = queueSystem;
            Number = number;
            IsEmpty = true;
        }

        #region PUBLICS
        public void QueueIsReleased()
        {
            IsEmpty = true;
            _queueSystem.AddQueuePoint(this);
        }
        public void QueueIsTaken()
        {
            IsEmpty = false;
            _queueSystem.RemoveQueuePoint(this);
        }
        #endregion
    }
}
