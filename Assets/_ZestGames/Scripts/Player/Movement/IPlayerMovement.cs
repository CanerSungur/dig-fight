
namespace ZestGames
{
    public interface IPlayerMovement
    {
        public void Init(Player player);
        public void Motor();
        public bool IsMoving { get; }
    }
}
