using UnityEngine;
using ZestGames;
using System;

namespace DigFight
{
    public class Pickaxe : MonoBehaviour
    {
        [Header("-- PICKAXE DATA --")]
        [SerializeField] private PickaxeBase _pickaxeBase;
        public PickaxeBase PickaxeBase => _pickaxeBase;

        private PlayerDigHandler _playerDigHandler;
        private AiDigHandler _aiDigHandler;

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
        private PickaxeStats _stats;
        public PickaxeStats Stats => _stats == null ? _stats = GetComponent<PickaxeStats>() : _stats;
        #endregion

        #region PROPERTIES
        public bool IsBroken { get; private set; }
        public bool CanHit { get; private set; }
        public Player Player => _playerDigHandler.Player;
        public Ai Ai => _aiDigHandler.Ai;
        #endregion

        #region EVENTS
        public Action OnCanHit, OnCannotHit, OnBreak;
        #endregion

        public void Init(PlayerDigHandler playerDigHandler)
        {
            if (_playerDigHandler == null)
            {
                _playerDigHandler = playerDigHandler;
                InitializeParticles();

                Stats.Init(this);
                DamageHandler.Init(this, true);
                DurabilityHandler.Init(this, true);
            }

            CanHit = IsBroken = false;
            EnableDefaultPickaxe();

            PlayerEvents.OnStartDigging += SelectRelevantPickaxe;
            PlayerEvents.OnStopDigging += EnableDefaultPickaxe;

            OnCanHit += EnableCanHit;
            OnCannotHit += DisableCanHit;
            OnBreak += Break;
        }
        public void Init(AiDigHandler aiDigHandler)
        {
            if (_aiDigHandler == null)
            {
                _aiDigHandler = aiDigHandler;
                InitializeParticles();

                DamageHandler.Init(this, false);
                DurabilityHandler.Init(this, false);
            }

            CanHit = IsBroken = false;
            EnableDefaultPickaxe();

            AiEvents.OnStartDigging += SelectRelevantPickaxe;
            AiEvents.OnStopDigging += EnableDefaultPickaxe;

            OnCanHit += EnableCanHit;
            OnCannotHit += DisableCanHit;
            OnBreak += Break;
        }

        private void OnDisable()
        {
            if (_playerDigHandler == null && _aiDigHandler == null) return;

            if (_playerDigHandler)
            {
                PlayerEvents.OnStartDigging -= SelectRelevantPickaxe;
                PlayerEvents.OnStopDigging -= EnableDefaultPickaxe;
            }
            else if (_aiDigHandler)
            {
                AiEvents.OnStartDigging -= SelectRelevantPickaxe;
                AiEvents.OnStopDigging -= EnableDefaultPickaxe;
            }

            OnCanHit -= EnableCanHit;
            OnCannotHit -= DisableCanHit;
            OnBreak -= Break;
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
                if (_playerDigHandler)
                {
                    GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.AskForRevive);
                    //PlayerEvents.OnLose?.Invoke();
                    //AiEvents.OnWin?.Invoke();
                }
                else if (_aiDigHandler)
                {
                    GameEvents.OnGameEnd?.Invoke(Enums.GameEnd.Success);
                    //PlayerEvents.OnWin?.Invoke();
                    //AiEvents.OnLose?.Invoke();
                }
                DisablePickaxe();
                CanHit = false;
            }
        }
        private void SelectRelevantPickaxe()
        {
            if (_playerDigHandler)
            {
                if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Top)
                    EnableDefaultPickaxe();
                else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Left)
                    EnableLeftPickaxe();
                else if (_playerDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Right)
                    EnableRightPickaxe();
            }
            else if (_aiDigHandler)
            {
                if (_aiDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Top)
                    EnableDefaultPickaxe();
                else if (_aiDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Left)
                    EnableLeftPickaxe();
                else if (_aiDigHandler.CurrentBoxTriggerDirection == Enums.BoxTriggerDirection.Right)
                    EnableRightPickaxe();
            }
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
