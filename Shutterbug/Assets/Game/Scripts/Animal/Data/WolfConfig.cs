using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "WolfConfig", menuName = "Game/Config/Wolf Config", order = 0)]
    public class WolfConfig : AnimalConfig
    {
        [field: SerializeField] public float DistanceToHit { get; private set; }
    }
}