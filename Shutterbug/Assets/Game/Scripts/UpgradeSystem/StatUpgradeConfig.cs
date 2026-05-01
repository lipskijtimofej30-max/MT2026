using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UpgradeSystem
{
    [CreateAssetMenu(fileName = "New Stat Upgrade Config", menuName = "Game/Config/Stat Upgrade Config")]
    public class StatUpgradeConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public UpgradeType Type { get; private set; }
        [SerializeField] private List<float> _values = new();
        [SerializeField] private List<float> _prices = new();
        
        public float GetValue(int level) => _values[Mathf.Clamp(level, 0, _values.Count - 1)];
        public float GetPrice(int level) => _prices[Mathf.Clamp(level, 0, _prices.Count - 1)];
        public int MaxLevel => _values.Count - 1;
    }
}