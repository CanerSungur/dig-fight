using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ChestPower : ChestBase
    {
        public override void Init(Layer layer)
        {
            base.Init(layer);

            powerUp = new PowerUp("POWER", duration, incrementValue);
        }

        public override void TriggerPickUp()
        {
            PowerUpEvents.OnPickaxePowerTaken?.Invoke(this, PowerUp);
        }
    }
}
