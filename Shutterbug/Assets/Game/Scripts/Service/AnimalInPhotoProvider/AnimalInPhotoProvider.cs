using System;

namespace Game.Scripts.Service
{
    public class AnimalInPhotoProvider : IAnimalInPhotoProvider
    {
        private BaseAnimalAI _bestTarget;

        public BaseAnimalAI TargetAnimal
        {
            get => _bestTarget;
            set
            {
                _bestTarget = value;
                OnTargetAnimalChanged?.Invoke(_bestTarget);
            }
        }

        public event Action<BaseAnimalAI> OnTargetAnimalChanged;
    }
}