using System;
using UnityEngine;

namespace Game.Scripts.Service
{
    public class AnimalInPhotoProvider : IAnimalInPhotoProvider
    {
        private CapturedPhotoData _bestTarget;

        public CapturedPhotoData LastPhotoData
        {
            get => _bestTarget;
            set
            {
                _bestTarget = value;
                OnCaptureDataChanged?.Invoke(_bestTarget);
            }
        }

        public event Action<CapturedPhotoData> OnCaptureDataChanged;
    }
}