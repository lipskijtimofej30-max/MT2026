using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.CameraPhoto.PhotoAlbum
{
    public class PhotoRegistry
    {
        private List<PhotoRecord> _photos = new();
        public IReadOnlyList<PhotoRecord> Photos => _photos;
        public int Count => _photos.Count;
        
        public void Register(PhotoRecord photoRecord) => _photos.Add(photoRecord);
        public void Unregister(PhotoRecord photoRecord) => _photos.Remove(photoRecord);
        public void Unregister(int index) => _photos.RemoveAt(index);

    }

    public class PhotoRecord
    {
        public Texture2D thumbnail;
        public AnimalType animalType;
        public AnimalState animalStateType;
        public PhotoScore photoScore;
        public string timestamp;
        public bool isSubmitted;

        public PhotoRecord(Texture2D thumbnail, AnimalType animalType, AnimalState animalStateType, PhotoScore photoScore, string timestamp, bool isSubmitted)
        {
            this.thumbnail = thumbnail;
            this.animalType = animalType;
            this.animalStateType = animalStateType;
            this.photoScore = photoScore;
            this.timestamp = timestamp;
            this.isSubmitted = isSubmitted;
        }
    }
}
