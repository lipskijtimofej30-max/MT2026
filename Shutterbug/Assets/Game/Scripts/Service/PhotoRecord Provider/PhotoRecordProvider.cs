using System;
using Game.Scripts.CameraPhoto.PhotoAlbum;

namespace Game.Service
{
    public class PhotoRecordProvider : IPhotoRecordProvider
    {
        private PhotoRecord _record;
        public PhotoRecord CurrentPhotoRecord
        {
            get => _record;
            set
            {
                _record = value;
                OnPhotoRecordChanged?.Invoke(_record);
            }
        }
        public event Action<PhotoRecord> OnPhotoRecordChanged;
    }
}