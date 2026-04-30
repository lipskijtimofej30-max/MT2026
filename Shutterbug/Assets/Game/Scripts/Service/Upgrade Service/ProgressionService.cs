using System;
using Game.Signals;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Service
{
    public class ProgressionService : IProgressionService
    {
        private const string SaveKey = "CameraLevel";
        
        private CameraUpgradesConfig _config;
        private SignalBus _signalBus;
        private int _currentLevel = 0;
        
        public int CurrentLevel => _currentLevel;
        public UpgradeLevel CurrentLevelData => _config.Levels[_currentLevel];

        [Inject]
        private void Construct(CameraUpgradesConfig config, SignalBus signalBus)
        {
            _config = config;
            _signalBus = signalBus;
            _currentLevel = PlayerPrefs.GetInt(SaveKey, 0);
        }
        
        public void LevelUp(UpgradeLevel level = null)
        {
            if (_currentLevel < _config.Levels.Count - 1)
            {
                _currentLevel++;
                PlayerPrefs.SetInt(SaveKey, _currentLevel);
                _signalBus.Fire(new LevelChangeSignal(CurrentLevelData.maxSlots));
            }
        }

        public void LevelDown(UpgradeLevel level = null)
        {
            if (_currentLevel > 0)
            {
                _currentLevel--;
                PlayerPrefs.SetInt(SaveKey, _currentLevel);
                _signalBus.Fire(new LevelChangeSignal(CurrentLevelData.maxSlots));
            }
        }
    }
}