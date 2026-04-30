using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public interface IAnimalFactory
    {
        BaseAnimalBrain Spawn(Vector3 position, BaseAnimalBrain prefab);
    }
}