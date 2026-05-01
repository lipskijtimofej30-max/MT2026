using System;
using System.Collections.Generic;
using Game.Scripts.UpgradeSystem;
using Game.Service.Currency;
using Game.Signals;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Service
{
    public class ProgressionService : IProgressionService
    {
        private Dictionary<UpgradeType, int> _levels = new();
        private StatUpgradesDatabase _configs;
        
        private ICurrencyService _currencyService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(StatUpgradesDatabase configs, ICurrencyService currencyService, SignalBus signalBus)
        {
            _configs = configs;
            _signalBus = signalBus;
            _currencyService = currencyService;
            Load();
        }

        public int GetLevel(UpgradeType type) => _levels.GetValueOrDefault(type, 0);
        public float GetCurrentValue(UpgradeType type)
        {
            int level = GetLevel(type);
            var config = _configs.Databased.Find(c => c.Type== type);
            return config.GetValue(level);
        }

        public void TryUpgrade(UpgradeType type, float playerMoney)
        {
            var config = _configs.Databased.Find(c => c.Type== type);
            int currentLevel = GetLevel(type);

            if (currentLevel < config.MaxLevel)
            {
                float price = config.GetPrice(currentLevel + 1);
                if (playerMoney >= price)
                {
                    _levels[type] = currentLevel + 1;
                    Save(type);
                
                    // Оповещаем систему о смене конкретного стата
                    _signalBus.Fire(new StatUpgradeSignal(type, config.GetValue(_levels[type])));
                }
            }
        }
        
        public StatUpgradeConfig GetConfig(UpgradeType type)
        {
            return _configs.Databased.Find(c => c.Type == type);
        }

        private void Save(UpgradeType type) => PlayerPrefs.SetInt($"Level_{type}", _levels[type]);
        private void Load() 
        {
            foreach (var type in (UpgradeType[])Enum.GetValues(typeof(UpgradeType)))
                _levels[type] = PlayerPrefs.GetInt($"Level_{type}", 0);
        }
    }
}
