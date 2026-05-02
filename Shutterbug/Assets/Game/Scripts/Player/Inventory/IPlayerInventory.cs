using Game.Scripts.Data;

namespace Game.Scripts
{
    public interface IPlayerInventory
    {
        void AddItem(Item item, int amount = 1);
        void RemoveItem(Item item, int amount = 1);
        bool Has(Item item, int amount = 1);
        int GetCount(Item item);
        void Clear();
    }
}