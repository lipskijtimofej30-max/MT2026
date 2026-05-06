using System;
using Game.Scripts.Service;
using Game.Scripts.UpgradeSystem;
using Game.Service;
using Game.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Shop
{
    public class ShopCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Button _button;
        private StatUpgradeConfig _config;
        private IProgressionService _progressionService;
        private ShopService _shopService;
        private SignalBus _bus;
        private PlayerBaseHandler _playerBaseHandler;

        public void Initialize(StatUpgradeConfig config, IProgressionService progressionService, ShopService shopService, SignalBus bus, PlayerBaseHandler playerBaseHandler)
        {
            _config = config;
            _progressionService = progressionService;
            _shopService = shopService;
            _bus = bus;
            _playerBaseHandler = playerBaseHandler;
            
            _button.interactable = _playerBaseHandler.InBase && _shopService.CanUpgrade(_config.Type);

            SetInfoText(_config);
            _bus.Subscribe<StatUpgradeSignal>(UpgradeCurrentStat);
            _button.onClick.AddListener(() => _shopService.Upgrade(_config.Type));
        }

        private void SetInfoText(StatUpgradeConfig config)
        {
            _infoText.text = $"{config.Name}\n" +
                             $"Значение: {_progressionService.GetCurrentValue(_config.Type)} -> {config.GetValue(_progressionService.GetLevel(config.Type)+1)}\n" +
                             $"Уровень: {_progressionService.GetLevel(_config.Type)}/{config.MaxLevel}\n" +
                             $"Цена для следущего уровня: {config.GetPrice(_progressionService.GetLevel(config.Type)+1)}";
        }

        private void UpgradeCurrentStat(StatUpgradeSignal signal)
        {
            if (signal.Type == _config.Type)
            {
                SetInfoText(_config);
            }
        }
    }
}