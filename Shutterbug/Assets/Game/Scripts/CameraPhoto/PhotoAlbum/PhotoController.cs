using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoController :MonoBehaviour
    {
        [SerializeField] private int _maxSlots = 20;
        private PhotoRegistry _photoRegistry;
        private PhotoRecord _currentPhotoRecord;

        public bool IsFull => _photoRegistry.Count >= _maxSlots;
        public PhotoRecord CurrentPhotoRecord => _currentPhotoRecord;
        public event Action OnPhotoAlbumChanged;

        [Inject]
        private void Construct(PhotoRegistry photoRegistry)
        {
            _photoRegistry = photoRegistry;
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

        public void DeletePhoto(int index)
        {
            if (index >= 0 && index < _photoRegistry.Count)
            {
                var record = _photoRegistry.Photos[index];
                Destroy(record.thumbnail);
                _photoRegistry.Unregister(index);
                record.thumbnail = null;
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