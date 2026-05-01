using System.Collections.Generic;
using Game.Scripts;
using Game.Scripts.Data;
using Game.Scripts.Service;
using Game.Service.Currency;
using UnityEngine;
using Zenject;

namespace Game.Service
{
    public class PhotoRewardService
    {
        private HashSet<AnimalType> _discoveredSpecies = new();
        private HashSet<(AnimalType, AnimalState)> _discoveredBehaviours = new();
        private PhotoRewardConfig _config;
        private IAnimalInPhotoProvider _provider;
        private ICurrencyService _currencyService;

        [Inject]
        private void Construct(IAnimalInPhotoProvider provider, ICurrencyService currencyService)
        {
            _provider = provider;
            _currencyService = currencyService;

            _provider.OnCaptureDataChanged += CheckPhotoData;
        }

        public PhotoRewardService(PhotoRewardConfig config)
        {
            _config = config;
        }

        private void CheckPhotoData(CapturedPhotoData data)
        {
            if (!_discoveredSpecies.Contains(data.AnimalType))
            {
                _discoveredSpecies.Add(data.AnimalType);
                _currencyService.AddCurrency(_config.NewSpeciesReward);
                Debug.LogWarning($"Открыто новое животное {data.AnimalType}");
            }

            if (!_discoveredBehaviours.Contains((data.AnimalType, data.AnimalState.StateType)))
            {
                _discoveredBehaviours.Add((data.AnimalType, data.AnimalState.StateType));
                Debug.LogWarning($"У животного {data.AnimalType} открыто новое состояние {data.AnimalState}");
                _currencyService.AddCurrency((int)(_config.NewBehaviorReward * data.AnimalState.GetStateMultiplier()));
            }
        }
    }
}