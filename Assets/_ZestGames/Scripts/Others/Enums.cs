namespace ZestGames
{
    public class Enums
    {
        public enum GameState { WaitingToStart, Started, PlatrofmEnded, GameEnded }
        public enum GameEnd { None, Success, Fail }
        public enum PoolStamp { Something, MoneyCollect2D, MoneySpend2D, HitBoxEffect, HitBoxSmoke }
        public enum PrefabStamp { DefaultBox, StaticBox, PushableBox, ExplosiveBox }
        public enum AudioType { Testing_PlayerMove, Button_Click, UpgradeMenu, CollectMoney, SpendMoney, HitBox, HitExplosive, Land, Swing, BreakBox }
        public enum AiStateType { Idle, WalkRandom, GetInQueue }
        public enum MovementDirection { None, Left, Right }
        public enum BoxTriggerDirection { Top, Left, Right }
    }
}
