using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "Bait", menuName = "Game/Items/Bait", order = 0)]
    public class BaitItem : Item
    {
        [field: SerializeField] public Bait BaitPrefab { get; set; }
    }
}