using UnityEngine;

namespace DigFight
{
    public interface IHealth
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; set; }
    }
}
