using Game.Scripts;
using UnityEngine;

namespace Game.Data
{
    public abstract class AnimalConfig : ScriptableObject
    {
        [field: SerializeField] public float ViewDistance { get; set; }
        [field: SerializeField] public float ViewAngle { get; set; }
        [field: SerializeField] public float ToSpecialStateDistance { get; set; }
        [field: SerializeField] public float WalkRadius { get; set; }
        [field: SerializeField] public ValueMinMax IdleTime { get; set; }
        [field: SerializeField] public ValueMinMax AlertTime { get; set; }
        [field: SerializeField] public float SmellRadius { get; set; }
        [field: SerializeField] public BaitType BaitType { get; set; }
    }
    
    [System.Serializable]
    public struct ValueMinMax
    {
        public float Min;
        public float Max;

        public ValueMinMax(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
