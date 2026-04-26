using System;

namespace Game.Scripts.Service
{
    public interface IAnimalInPhotoProvider
    {
        CapturedPhotoData LastPhotoData { get; set; }
        event Action<CapturedPhotoData> OnCaptureDataChanged;
    }

    public class CapturedPhotoData
    {
        public AnimalType AnimalType;
        public AnimalState  AnimalState;

        public CapturedPhotoData(AnimalType animalType, AnimalState animalState)
        {
            AnimalType = animalType;
            AnimalState = animalState;
        }
    }
}