using System;
using Game.Scripts.CameraPhoto.PhotoAlbum;

namespace Game.Service
{
    public interface IPhotoRecordProvider
    {
        PhotoRecord CurrentPhotoRecord {get; set;}
        public event Action<PhotoRecord> OnPhotoRecordChanged;
    }
}