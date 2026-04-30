using System;

namespace Game.Scripts.Service
{
    public interface IProgressionService
    {
        int CurrentLevel { get; }
        UpgradeLevel CurrentLevelData { get; }
        void LevelUp(UpgradeLevel level);
        void LevelDown(UpgradeLevel level);
    }
}