using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class Pickaxe : MonoBehaviour
    {
        private PlayerDigHandler _playerDigHandler;

        [Header("-- VISUAL SETUP --")]
        [SerializeField] private GameObject defaultPickaxe;
        [SerializeField] private GameObject leftPickaxe;
        [SerializeField] private GameObject rightPickaxe;

        private ParticleSystem _defaultPickaxeHitPS, _leftPickaxeHitPS, _rightPickaxeHitPS;

        #region SCRIPT REFERENCES
        private PickaxeDamageHandler _damageHandler;
        public PickaxeDamageHandler DamageHandler => _damageHandler == null ? _damageHandler = GetComponent<PickaxeDamageHandler>() : _damageHandler;
        private PickaxeDurabilityHandler _durabilityHandler;
        public PickaxeDurabilityHandler DurabilityHandler => _durabilityHandler == null ? _durabilityHandler = GetComponent<PickaxeDurabilityHandler>() : _durabilityHandler;
        #endregion

        #region PROPERTIES
        public bool IsBroken { get; private set; }
        public bool CanHit { get; private set; }
        public Player Player => _playerDigHandler.Player;
        #endregion

        public void Init(PlayerDigHandler playerDigHandler)
        {
            if (_playerDigHandler == null)
            {
                _playerDigHandler = playerDigHandler;
                InitializeParticles();

                DamageHandler.Init(this);
                DurabilityHandler.Init(this);
            }

            CanHit = IsBroken = false;
            EnableDefaultPickaxe();

            PlayerEvents.OnStartDigging += SelectRelevantPickaxe;
            PlayerEvents.OnStopDigging += EnableDefaultPickaxe;

            PickaxeEvents.OnCanHit += EnableCanHit;
            PickaxeEvents.OnCannotHit += DisableCanHit;
            PickaxeEvents.OnBreak += Break;
        }

        private void OnDisable()
        {
            if (_playerDigHandler == null) return;
            PlayerEvents.OnStartDigging -= SelectRelevantPickaxe;
            PlayerEvents.OnStopDigging -= EnableDefaultPickaxe;

            PickaxeEvents.OnCanHit -= EnableCanHit;
            PickaxeEvents.OnCannotHit -= DisableCanHit;
            PickaxeEvents.OnBreak -= Break;
        }

        #region EVENT HANDLER FUNCTIONS
        private void EnableCanHit()
        {
            CanHit = true;
        }
        private void DisableCanHit()
        {
            CanHit = false;
        }
        private void Break()
        {
            IsBroken = true;
            if (GameManager.GameState != Enums.GameState.GameEnded)
            {
                GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Fail);
                PlayerEvents.OnLose?.Invoke();
                DisablePickaxe();
                CanHit = false;
            }
        }
        private void SelectRelevantPickaxe()
        {
            if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Top)
                EnableDefaultPickaxe();
            else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Left)
                EnableLeftPickaxe();
            else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Right)
                EnableRightPickaxe();
        }
        #endregion

        #region HELPERS
        private void EnableDefaultPickaxe()
        {
            defaultPickaxe.SetActive(true);
            leftPickaxe.SetActive(false);
            rightPickaxe.SetActive(false);
        }
        private void EnableLeftPickaxe()
        {
            defaultPickaxe.SetActive(false);
            leftPickaxe.SetActive(true);
            rightPickaxe.SetActive(false);
        }
        private void EnableRightPickaxe()
        {
            defaultPickaxe.SetActive(false);
            leftPickaxe.SetActive(false);
            rightPickaxe.SetActive(true);
        }
        private void DisablePickaxe()
        {
            defaultPickaxe.SetActive(false);
            leftPickaxe.SetActive(false);
            rightPickaxe.SetActive(false);
        }
        private void InitializeParticles()
        {
            _defaultPickaxeHitPS = defaultPickaxe.transform.GetChild(0).GetComponent<ParticleSystem>();
            _leftPickaxeHitPS = leftPickaxe.transform.GetChild(0).GetComponent<ParticleSystem>();
            _rightPickaxeHitPS = rightPickaxe.transform.GetChild(0).GetComponent<ParticleSystem>();
        }
        #endregion

        #region PUBLICS
        public void HitHappened()
        {
            if (defaultPickaxe.activeSelf)
                PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, defaultPickaxe.transform.position, Quaternion.identity);
            else if (leftPickaxe.activeSelf)
                PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, leftPickaxe.transform.position, Quaternion.identity);
            else if (rightPickaxe.activeSelf)
                PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.HitBoxEffect, rightPickaxe.transform.position, Quaternion.identity);
        }
        #endregion
    }
}
