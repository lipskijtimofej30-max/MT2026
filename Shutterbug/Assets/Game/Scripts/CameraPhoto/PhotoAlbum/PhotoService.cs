using System;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoService
    { 
        private int _maxSlots;
        private PhotoRegistry _photoRegistry;
        private PhotoRecord _currentPhotoRecord;

        public bool IsFull => _photoRegistry.Count >= _maxSlots;
        public PhotoRecord CurrentPhotoRecord => _currentPhotoRecord;
        public event Action OnPhotoAlbumChanged;

        [Inject]
        private void Construct(PhotoRegistry photoRegistry, IProgressionService progressionService)
        {
            _photoRegistry = photoRegistry;
            _maxSlots = progressionService.CurrentLevelData.maxSlots;
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
            Debug.LogWarning("Current photo is " + _currentPhotoRecord);
            OnPhotoAlbumChanged?.Invoke();
        }
    }
}
