using UnityEngine;

namespace DigFight
{
    public interface IBoxInteractable
    {
        public void Init(Layer layer);
        public void ChangeParent(Transform newParent);
    }
}
