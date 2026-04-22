using System;
using UnityEngine;

namespace Game.Scripts.Service
{
    public class PhotoProvider: IPhotoProvider
    {
        private Texture2D _texture;
        private PhotoScore _score;
        public PhotoScore Score 
        { 
            get => _score;
            set
            {
                if(_score == value) return;
                _score = value;
                TryInvokeCombinedEvent();
            }
        }

        public Texture2D Photo { 
            get => _texture;
            set
            {
                if (_texture == value) return;
                _texture = value;
                TryInvokeCombinedEvent();
            }
        }

        public event Action<Texture2D, PhotoScore> OnPhotoAndScoreReady;
        private void TryInvokeCombinedEvent()
        {
            if (_texture != null && _score != null)
            {
                OnPhotoAndScoreReady?.Invoke(_texture, _score);
            }
        }
    }
}