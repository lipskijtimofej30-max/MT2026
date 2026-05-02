using System.Collections.Generic;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts
{
    public class PlayerInventory : IPlayerInventory
    {
        private Dictionary<Item, int> _items = new();

        public void AddItem(Item item, int amount = 1)
        {
            if (!_items.ContainsKey(item))
                _items[item] = amount;
            else
                _items[item] += amount;
            Debug.LogWarning($"{item.Name} has been added to player inventory");
        }

        public void RemoveItem(Item item, int amount = 1)
        {
            if (!_items.ContainsKey(item)) return;
            _items[item] -= amount;
            Debug.LogWarning($"{item.Name} has been removed from player inventory");
        }

        public bool Has(Item item, int amount = 1)
        {
            if (!_items.TryGetValue(item, out int currentAmount))
                return false;

            return currentAmount >= amount;
        }

        public int GetCount(Item item) => _items.TryGetValue(item, out int count) ? count : 0;

        public void Clear()
        {
            _items.Clear();
        }
    }
}
