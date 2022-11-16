using System;

namespace ZestGames
{
    public static class EventManager { }

    public static class GameEvents 
    {
        public static Action OnGameStart, OnLevelSuccess, OnLevelFail, OnChangePhase;
        public static Action<Enums.GameEnd> OnGameEnd, OnChangeScene;
    }

    public static class PlayerEvents 
    {
        public static Action OnMove, OnIdle, OnDie, OnWin, OnLose, OnCheer;
        public static Action OnSetCurrentMovementSpeed, OnSetCurrentMoneyValue;
        public static Action OnOpenedUpgradeCanvas, OnClosedUpgradeCanvas;
        public static Action OnEmptyNextInQueue, OnStopSpendingMoney;
    }

    public static class PlayerUpgradeEvents
    {
        public static Action OnOpenCanvas, OnCloseCanvas, OnUpdateUpgradeTexts, OnUpgradeMovementSpeed, OnUpgradeMoneyValue;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateLevelText;
        public static Action<float> OnUpdateCollectableText;
        public static Action<string, FeedBackUi.Colors> OnGiveFeedBack;
    }

    public static class CollectableEvents
    {
        public static Action<float> OnCollect, OnSpend;
    }
    
    public static class InputEvents
    {
        public static Action OnTapHappened, OnTouchStarted, OnTouchStopped;
    }

    public static class AudioEvents
    {
        public static Action OnPlayCollectMoney, OnPlaySpendMoney;
    }
}
