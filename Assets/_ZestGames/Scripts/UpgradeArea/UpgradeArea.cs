namespace ZestGames
{
    public class UpgradeArea : UpgradeAreaBase
    {
        public override void OpenUpgradeCanvas()
        {
            if (!UpgradeCanvas.IsOpen)
                PlayerUpgradeEvents.OnOpenCanvas?.Invoke();
        }

        public override void CloseUpgradeCanvas()
        {
            if (UpgradeCanvas.IsOpen)
                PlayerUpgradeEvents.OnCloseCanvas?.Invoke();
        }
    }
}
