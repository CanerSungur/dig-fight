using UnityEngine;
using ZestGames;
using System.Collections;

namespace DigFight
{
    public class PlayerPowerUpHandler : MonoBehaviour
    {
        private Player _player;

        #region DURATION
        private WaitForSeconds _waitForSpeedDuration, _waitForPowerDuration;
        #endregion

        #region BOOST RATES
        private float _speedRate;
        private int _powerRate;
        #endregion

        #region COROUTINES
        private IEnumerator _speedEnumerator, _powerEnumerator = null;
        #endregion

        #region PROPERTIES
        public float SpeedRate => _speedRate;
        public int PowerRate => _powerRate;
        #endregion

        public void Init(Player player)
        {
            if (_player == null)
                _player = player;

            _speedRate = _powerRate = 0;

            PlayerEvents.OnActivatePickaxeSpeed += ActivatePickaxeSpeed;
            PlayerEvents.OnActivatePickaxePower += ActivatePickaxePower;
        }

        private void OnDisable()
        {
            if (_player == null) return;

            PlayerEvents.OnActivatePickaxeSpeed -= ActivatePickaxeSpeed;
            PlayerEvents.OnActivatePickaxePower -= ActivatePickaxePower;
        }

        #region EVENT HANDLER FUNCTIONS
        private void ActivatePickaxeSpeed(PowerUp powerUp)
        {
            StopPickaxeSpeed();
            StartPickaxeSpeed(powerUp);
        }
        private void ActivatePickaxePower(PowerUp powerUp)
        {
            StopPickaxePower();
            StartPickaxePower(powerUp);
        }
        #endregion

        #region COROUTINES
        #region SPEED
        private void StartPickaxeSpeed(PowerUp powerUp)
        {
            if (_speedEnumerator == null)
            {
                _waitForSpeedDuration = new WaitForSeconds(powerUp.Duration);
                _speedEnumerator = PickaxeSpeedCoroutine(powerUp);
                StartCoroutine(_speedEnumerator);
            }
        }
        private void StopPickaxeSpeed()
        {
            if (_speedEnumerator == null) return;
            StopCoroutine(_speedEnumerator);
            _speedEnumerator = null;
        }
        private IEnumerator PickaxeSpeedCoroutine(PowerUp powerUp)
        {
            _speedRate = powerUp.IncrementValue;
            PlayerEvents.OnSetCurrentPickaxeSpeed?.Invoke();
            _player.AnimationController.StartScaleSequence(powerUp.Duration);

            yield return _waitForSpeedDuration;

            _speedRate = 0;
            PlayerEvents.OnSetCurrentPickaxeSpeed?.Invoke();
            _player.EffectHandler.StopPickaxeSpeed();
            StopPickaxeSpeed();
        }
        #endregion

        #region POWER
        private void StartPickaxePower(PowerUp powerUp)
        {
            if (_powerEnumerator == null)
            {
                _waitForPowerDuration = new WaitForSeconds(powerUp.Duration);
                _powerEnumerator = PickaxePowerCoroutine(powerUp);
                StartCoroutine(_powerEnumerator);
            }
        }
        private void StopPickaxePower()
        {
            if (_powerEnumerator == null) return;
            StopCoroutine(_powerEnumerator);
            _powerEnumerator = null;
        }
        private IEnumerator PickaxePowerCoroutine(PowerUp powerUp)
        {
            _powerRate = (int)powerUp.IncrementValue;
            PlayerEvents.OnSetCurrentPickaxePower?.Invoke();
            _player.AnimationController.StartScaleSequence(powerUp.Duration);

            yield return _waitForPowerDuration;

            _powerRate = 0;
            PlayerEvents.OnSetCurrentPickaxePower?.Invoke();
            _player.EffectHandler.StopPickaxePower();
            StopPickaxePower();
        }
        #endregion

        #endregion
    }
}
