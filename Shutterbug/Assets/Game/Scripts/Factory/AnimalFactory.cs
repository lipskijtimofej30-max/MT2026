using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Factory
{
    public class AnimalFactory: MonoBehaviour, IAnimalFactory
    {
        [SerializeField] private BaseAnimalAI _prefab;
        private DiContainer _container;
        private AnimalRegistry _animalRegistry;
        
        [Inject]
        private void Construct(DiContainer container, AnimalRegistry animalRegistry)
        {
            _container = container;
            _animalRegistry = animalRegistry;
        }

        public BaseAnimalAI Spawn(Vector3 position)
        {
            var obj = _container.InstantiatePrefabForComponent<BaseAnimalAI>(_prefab.gameObject, position, Quaternion.identity, transform);
            _animalRegistry.Register(obj);
            Debug.Log($"Spawn animal: {obj.name}");
            return obj;
        }
    }
}
