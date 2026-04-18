using System;
using UnityEngine;

namespace Game.Scripts.Service
{
    public class PhotoProvider: IPhotoProvider
    {
        private Texture2D _texture;
        public Texture2D Photo { 
            get => _texture;
            set
            {
                if (_texture == value) return;
                _texture = value;
                OnPhotoLoaded?.Invoke(_texture);
            }
        }
        public event Action<Texture2D> OnPhotoLoaded;
    }
}