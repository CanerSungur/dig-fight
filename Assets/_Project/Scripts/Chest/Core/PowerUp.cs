using UnityEngine;

namespace DigFight
{
    public class PowerUp
    {
        private string _name;
        private float _duration;
        private float _incrementValue;

        public PowerUp(string name, float duration, float incrementValue)
        {
            _name = name;
            _duration = duration;
            _incrementValue = incrementValue;
        }

        #region GETTERS
        public string Name => _name;
        public float Duration => _duration;
        public float IncrementValue => _incrementValue;
        #endregion
    }
}
