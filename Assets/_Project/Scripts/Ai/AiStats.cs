using ZestGames;

namespace DigFight
{
    public static class AiStats 
    {
        #region CORE DATA
        private const int CORE_PICAXE_POWER = 1;
        private const int CORE_PICKAXE_DURABILITY = 15;
        private const float CORE_PICKAXE_SPEED = 1f;
        #endregion

        #region INCREMENT DATA
        private const int PICKAXE_POWER_INCREMENT = 1;
        private const int PICKAXE_DURABILITY_INCREMENT = 2;
        private const float PICKAXE_SPEED_INCREMENT = 0.1f;
        #endregion

        public static int PickaxePower => CORE_PICAXE_POWER + (LevelHandler.Level * PICKAXE_POWER_INCREMENT);
        public static int PickaxeDurability => CORE_PICKAXE_DURABILITY + (LevelHandler.Level * PICKAXE_DURABILITY_INCREMENT);
        public static float PickaxeSpeed => CORE_PICKAXE_SPEED + (LevelHandler.Level * PICKAXE_SPEED_INCREMENT);
    }
}
