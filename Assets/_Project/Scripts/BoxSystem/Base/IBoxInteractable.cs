using UnityEngine;
using ZestGames;

namespace DigFight
{
    public interface IBoxInteractable
    {
        public void Init(Layer layer);
        public void ChangeParent(Transform newParent);
        public void AssignInteracter(Player player);
        public void AssignInteracter(Ai ai);
    }
}
