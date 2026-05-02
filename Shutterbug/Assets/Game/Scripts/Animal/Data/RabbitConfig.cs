using Game.Scripts;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "RabbitConfig", menuName = "Game/Config/RabbitConfig")]
    public class RabbitConfig : AnimalConfig
    {
        [field: SerializeField] public ValueMinMax AlertTime { get; set; }
        [field: SerializeField] public BaitType BaitType { get; set; }
        [field: SerializeField] public float SmellRadius { get; set; }
    }
}