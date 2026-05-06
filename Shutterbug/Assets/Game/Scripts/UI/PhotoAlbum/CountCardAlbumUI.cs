using System;
using Game.Scripts.CameraPhoto.PhotoAlbum;
using Game.Scripts.Service;
using Game.Scripts.UpgradeSystem;
using Game.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Photo
{
    public class CountCardAlbumUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text countText;
        private int _maxCount;
        private int _currentCount;
        
        private PhotoRegistry _photoRegistry;
        private IProgressionService _progressionService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(PhotoRegistry photoRegistry, IProgressionService progressionService, SignalBus signalBus)
        {
            _photoRegistry = photoRegistry;
            _signalBus = signalBus;
            _progressionService = progressionService;
            
            _maxCount = (int)progressionService.GetCurrentValue(UpgradeType.MaxAlbumSlots);
            SetText(_photoRegistry.Count);
            
            _signalBus.Subscribe<StatUpgradeSignal>(UpdateMaxCount);
            _photoRegistry.OnCountChanged += SetText;
        }
        
        private void OnDestroy()
        {
            _photoRegistry.OnCountChanged -= SetText;
        }

        private void SetText(int count)
        {
            _currentCount = count;
            countText.text = $"{count}/{_maxCount}";
        }
        
        private void UpdateMaxCount(StatUpgradeSignal signal)
        {
            if (signal.Type == UpgradeType.MaxAlbumSlots)
            {
                var count = (int)_progressionService.GetCurrentValue(UpgradeType.MaxAlbumSlots);
                _maxCount = count;
                SetText(_currentCount);
            }
        }
        
    }
}