using System.Collections.Generic;
using Game.Scripts.Factory;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<BaseAnimalBrain> _animals = new();
        [SerializeField] private List<Transform> _transformPoint;
        private IAnimalFactory _animalFactory;

        [Inject]
        private void Construct(IAnimalFactory animalFactory)
        {
            _animalFactory = animalFactory;
        }

        private void Start()
        {
            foreach (var point in _transformPoint)
            {
                var index = Random.Range(0, _animals.Count);
                _animalFactory.Spawn(point.position, _animals[index]);
            }
        }
    }
}