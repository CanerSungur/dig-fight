using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ChestSpeed : ChestBase
    {
        public override void Init(Layer layer)
        {
            base.Init(layer);

            powerUp = new PowerUp("SPEED", duration, incrementValue);
        }

        public override void TriggerPickUp()
        {
            PowerUpEvents.OnPickaxeSpeedTaken?.Invoke(this, PowerUp);
        }
    }
}
