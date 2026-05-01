using Game.Scripts.Core;
using Game.Scripts.UpgradeSystem;
using UnityEngine;

namespace Game.Scripts.Service
{
    [CreateAssetMenu(fileName = "StatUpgradesDatabase", menuName = "Game/Stat Upgrades Database")]
    public class StatUpgradesDatabase : Database<StatUpgradeConfig> { }
}