using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Factory
{
    public interface IAnimalFactory
    {
        BaseAnimalAI Spawn(Vector3 position);
    }
}