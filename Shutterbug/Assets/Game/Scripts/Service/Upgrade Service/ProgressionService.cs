using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Service
{
    public class ProgressionService : IProgressionService
    {
        private CameraUpgradesConfig _config;
        private int _currentLevel = 0;
        private const string SaveKey = "CameraLevel";
        
        public int CurrentLevel => _currentLevel;
        public UpgradeLevel CurrentLevelData => _config.Levels[_currentLevel];

        [Inject]
        private void Construct(CameraUpgradesConfig config)
        {
            _config = config;
            _currentLevel = PlayerPrefs.GetInt(SaveKey, 0);
        }

        
        public void LevelUp(UpgradeLevel level)
        {
            if (_currentLevel < _config.Levels.Count - 1)
            {
                _currentLevel++;
                PlayerPrefs.SetInt(SaveKey, _currentLevel);
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }

        public void LevelDown(UpgradeLevel level)
        {
            if (_currentLevel > 0)
            {
                _currentLevel--;
                PlayerPrefs.SetInt(SaveKey, _currentLevel);
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }

        public event Action<int> OnLevelChanged;
    }
}