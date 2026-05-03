using UnityEngine;

namespace Game.Scripts.Data
{
    public abstract class Item : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public float Cost { get; set; }
        [field: SerializeField] public int MaxStack { get; set; } 
    }
}