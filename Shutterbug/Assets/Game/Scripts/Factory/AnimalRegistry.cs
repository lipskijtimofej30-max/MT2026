using System.Collections.Generic;

namespace Game.Scripts.Factory
{
    public class AnimalRegistry
    {
        private List<BaseAnimalBrain> _animals = new();
        public IReadOnlyList<BaseAnimalBrain> Animals => _animals;
        
        public void Register(BaseAnimalBrain animal) => _animals.Add(animal);
        public void Unregister(BaseAnimalBrain animal) => _animals.Remove(animal);
    }
}