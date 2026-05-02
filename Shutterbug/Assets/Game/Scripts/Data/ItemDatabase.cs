using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "Game/Item Database", order = 0)]
    public class ItemDatabase : Database<Item> { }
}