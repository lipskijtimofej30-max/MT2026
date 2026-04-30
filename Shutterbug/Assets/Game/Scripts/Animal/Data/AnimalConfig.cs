using UnityEngine;

namespace Game.Data
{
    public abstract class AnimalConfig : ScriptableObject
    {
        [field: SerializeField] public float ViewDistance { get; set; }
        [field: SerializeField] public float ViewAngle { get; set; }
        [field: SerializeField] public float ToSpecialStateDistance { get; set; }
    }
}

