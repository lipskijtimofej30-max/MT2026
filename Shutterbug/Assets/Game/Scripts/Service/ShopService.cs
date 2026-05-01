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

        [Inject]
        private void Construct(ICurrencyService currencyService, IProgressionService progressionService)
        {
            _currencyService = currencyService;
            _progressionService =  progressionService;
        }
        
        public bool CanUpgrade(UpgradeType type)
        {
            var config = _progressionService.GetConfig(type);
            int currentLevel = _progressionService.GetLevel(type);
    
            if (currentLevel >= config.MaxLevel) return false;

            float price = config.GetPrice(currentLevel + 1);
            return _currencyService.Currency >= price; 
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