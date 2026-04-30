using System;
using Game.Scripts.CameraPhoto.PhotoAlbum;
using Game.Scripts.Service;
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
        
        private PhotoRegistry _photoRegistry;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(PhotoRegistry photoRegistry, IProgressionService progressionService, SignalBus signalBus)
        {
            _photoRegistry = photoRegistry;
            _signalBus = signalBus;
            
            _maxCount = progressionService.CurrentLevelData.maxSlots;
            SetText(_photoRegistry.Count);
            
            _signalBus.Subscribe<LevelChangeSignal>(UpdateMaxCount);
            _photoRegistry.OnCountChanged += SetText;
        }
        
        private void OnDestroy()
        {
            _photoRegistry.OnCountChanged -= SetText;
        }

        private void SetText(int count)
        {
            countText.text = $"{count}/{_maxCount}";
        }
        

        private void UpdateMaxCount(LevelChangeSignal signal)
        {
            Debug.LogError("UpdateMaxCount");
            var count = signal.Level;
            _maxCount = count;
        }
        
    }
}