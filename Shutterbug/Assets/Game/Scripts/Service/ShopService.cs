using System;
using Game.Scripts;
using Game.Scripts.Data;
using Game.Scripts.Service;
using Game.Scripts.UpgradeSystem;
using Game.Service.Currency;
using Zenject;

namespace Game.Service
{
    public class ShopService
    {
        private ICurrencyService _currencyService;
        private IProgressionService _progressionService;
        private IPlayerInventory _playerInventory;

        [Inject]
        private void Construct(ICurrencyService currencyService, IProgressionService progressionService, IPlayerInventory playerInventory)
        {
            _currencyService = currencyService;
            _progressionService =  progressionService;
            _playerInventory = playerInventory;
        }
        
        public bool CanUpgrade(UpgradeType type)
        {
            var config = _progressionService.GetConfig(type);
            int currentLevel = _progressionService.GetLevel(type);
    
            if (currentLevel >= config.MaxLevel) return false;

            float price = config.GetPrice(currentLevel + 1);
            return _currencyService.Currency >= price; 
        }

        public bool CanBuyItem(Item item)
        {
            int currentCount = _playerInventory.GetCount(item);
            bool hasMoney = _currencyService.Currency >= item.Cost;
            bool hasSpace = currentCount < item.MaxStack; // Проверка лимита

            return hasMoney && hasSpace;
        }

        public void BuyItem(Item item)
        {
            if (CanBuyItem(item))
            {
                _playerInventory.AddItem(item);
                _currencyService.RemoveCurrency((int)(item.Cost));
            }
        }

        public void Upgrade(UpgradeType type)
        {
            if (CanUpgrade(type))
            {
                _progressionService.TryUpgrade(type, _currencyService.Currency);
                _currencyService.RemoveCurrency((int)(_progressionService.GetConfig(type).GetPrice(_progressionService.GetLevel(type))));
            }
        }
    }
}