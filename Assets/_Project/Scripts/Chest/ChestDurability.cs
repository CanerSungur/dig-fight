using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ChestDurability : ChestBase
    {
        public override void Init(Layer layer)
        {
            base.Init(layer);

            powerUp = new PowerUp("DURABILITY", duration, incrementValue);
        }

        public override void TriggerPickUp()
        {
            PowerUpEvents.OnPickaxeDurabilityTaken?.Invoke(this, PowerUp);
        }
    }
}
