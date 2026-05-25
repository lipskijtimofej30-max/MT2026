using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Factory;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> _transformPoint;
        private List<BaseAnimalBrain> _animals = new();
        private IAnimalFactory _animalFactory;

        [Inject]
        private void Construct(IAnimalFactory animalFactory)
        {
            _animalFactory = animalFactory;
            _animals = Resources.LoadAll<BaseAnimalBrain>("Prefabs/Animal").ToList();
        }

        private void Start()
        {
            foreach (var animal in _animals)
            {
                var index = Random.Range(0, _transformPoint.Count);
                _animalFactory.Spawn(_transformPoint[index].position, animal, transform);
            }
        }
    }
}