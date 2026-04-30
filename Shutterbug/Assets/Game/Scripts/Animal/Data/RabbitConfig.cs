using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "RabbitConfig", menuName = "Game/Config/RabbitConfig")]
    public class RabbitConfig : AnimalConfig
    {
        [field: SerializeField] public ValueMinMax AlertTime { get; set; }
    }
}