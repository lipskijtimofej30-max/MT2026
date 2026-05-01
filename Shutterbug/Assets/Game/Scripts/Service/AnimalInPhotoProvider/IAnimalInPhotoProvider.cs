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
        public IState AnimalState;

        public CapturedPhotoData(AnimalType animalType, IState animalState)
        {
            AnimalType = animalType;
            AnimalState = animalState;
        }
    }
}