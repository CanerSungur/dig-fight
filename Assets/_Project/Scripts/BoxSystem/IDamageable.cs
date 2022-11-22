using UnityEngine;

namespace DigFight
{
    public interface IDamageable
    {
        public void GetDamaged(int amount);
        public void Break();
    }
}
