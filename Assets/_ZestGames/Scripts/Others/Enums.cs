namespace ZestGames
{
    public class Enums
    {
        public enum GameState { WaitingToStart, Started, PlatrofmEnded, GameEnded }
        public enum GameEnd { None, Success, Fail, AskForRevive }
        public enum PoolStamp { Something, MoneyCollect2D, MoneySpend2D, HitBoxEffect, HitBoxSmokeSquare, HitBoxSmoke, MoneyTextDisplay, ExplosionLargeEffect, VFX_Voxel_Stone, VFX_Voxel_Copper, VFX_Voxel_Diamond, Debris_Stone, Debris_Copper, Debris_Diamond, VFX_Chest_Break, CharacterPop_Confetti }
        public enum PrefabStamp { StoneBox, StaticBox, PushableBox, ExplosiveBox, MiddleBox, LevelEndBox, BorderBox, CopperBox, DiamondBox, ChestPickaxeDurability, ChestPickaxeSpeed }
        public enum AudioType { Testing_PlayerMove, Button_Click, UpgradeMenu, CollectMoney, SpendMoney, HitBox, HitExplosive, Land, Swing, BreakBox, PushBox, PushBoxDrop, HitChest, ChestOpen, BreakChest, CharacterPop }
        public enum AiStateType { Idle, WalkRandom, GetInQueue, Fly, Run, Dig, Push, Fall, Revive }
        public enum MovementDirection { None, Left, Right }
        public enum DigDirection { None, Left, Right, Down, Up }
        public enum BoxTriggerDirection { None, Left, Right, Top, Down }
        public enum BoxType { Stone, Copper, Diamond, TNT }
        public enum ChestType { PickaxeDurability, PickaxeSpeed, PickaxePower }
        public enum HitPower { Low, Medium, High }
        public enum LayerType { Breakable, Pushable, DurabilityChest, SpeedChest }
        public enum Surrounding { Empty, Wall, BreakableBox, ExplosiveBox, PushableBox, Chest, FinishLine }
    }
}
