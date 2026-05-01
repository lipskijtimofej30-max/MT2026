using System;
using Game.Scripts.Service;
using Game.Scripts.UpgradeSystem;
using Game.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoService
    { 
        private int _maxSlots;
        private PhotoRegistry _photoRegistry;
        private IPhotoRecordProvider _recordProvider;
        private PhotoRecord _currentPhotoRecord;

        public bool IsFull => _photoRegistry.Count >= _maxSlots;
        public PhotoRecord CurrentPhotoRecord => _currentPhotoRecord;
        public event Action OnPhotoAlbumChanged;

        [Inject]
        private void Construct(PhotoRegistry photoRegistry, IProgressionService progressionService, IPhotoRecordProvider recordProvider)
        {
            _photoRegistry = photoRegistry;
            _maxSlots = (int)progressionService.GetCurrentValue(UpgradeType.MaxAlbumSlots);
            _recordProvider = recordProvider;
        }
        
        public bool AddPhoto(PhotoRecord photo)
        {
            if (IsFull)
            {
                Debug.LogWarning("Photo is full");
                return false;
            }   
            _photoRegistry.Register(photo);
            OnPhotoAlbumChanged?.Invoke();
            return true;
        }

        public void DeletePhoto(PhotoRecord photo)
        {
            if (photo != null &&  _photoRegistry.Count > 0)
            {
                GameObject.Destroy(photo.thumbnail);
                _photoRegistry.Unregister(photo);
                photo.thumbnail = null;
                OnPhotoAlbumChanged?.Invoke();
            }
        }

        public void SetCurrentPhoto(PhotoRecord photo)
        {
            _currentPhotoRecord = photo;
            _recordProvider.CurrentPhotoRecord = photo;
            Debug.LogWarning("Current photo is " + _currentPhotoRecord);
            OnPhotoAlbumChanged?.Invoke();
        }
    }
}
