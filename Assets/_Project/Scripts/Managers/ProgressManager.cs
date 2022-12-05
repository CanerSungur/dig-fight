using System;
using UnityEngine;
using ZestGames;

namespace DigFight
{
    public class ProgressManager : MonoBehaviour
    {
        private GameManager _gameManager;

        #region LINE VISUAL
        [Header("-- LINE SETUP --")]
        [SerializeField] private ProgressLine _progressLine;
        #endregion

        #region DEPTH DATA RELATED
        private float _maxDepthAchieved;
        private float _currentDepthAchieved;
        #endregion

        #region PROPERTIES
        public float MaxDepthAchieved => _maxDepthAchieved;
        #endregion

        public void Init(GameManager gameManager)
        {
            if (_gameManager == null)
                _gameManager = gameManager;

            _currentDepthAchieved = 0;

            GameEvents.OnGameEnd += HandleGameEnd;

            Load();

            _progressLine.Init(this);
        }

        private void OnDisable()
        {
            if (_gameManager == null) return;
            GameEvents.OnGameEnd -= HandleGameEnd;

            Save();
        }

        private void Update()
        {
            _currentDepthAchieved = CharacterTracker.PlayerTransform.position.y;
        }

        private void InitalizeLine()
        {
            if (_maxDepthAchieved == 0) return;
            // put line on max depth
        }

        #region EVENT HANDLER FUNCTIONS
        private void HandleGameEnd(Enums.GameEnd gameEnd)
        {
            _currentDepthAchieved *= -1f;
            if (gameEnd == Enums.GameEnd.Success) _maxDepthAchieved = 0;
            else _maxDepthAchieved = _currentDepthAchieved > _maxDepthAchieved ? (_currentDepthAchieved < -0.9f ? 0 : _currentDepthAchieved) : _maxDepthAchieved;
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void Save()
        {
            PlayerPrefs.SetFloat("MaxDepthAchieved", _maxDepthAchieved);
            PlayerPrefs.Save();
        }
        private void Load()
        {
            _maxDepthAchieved = PlayerPrefs.GetFloat("MaxDepthAchieved", 0);
        }
        #endregion
    }
}
