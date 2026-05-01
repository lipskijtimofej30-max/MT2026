using System;
using Game.Scripts.UpgradeSystem;

namespace Game.Scripts.Service
{
    public interface IProgressionService
    {
        int GetLevel(UpgradeType type);
        float GetCurrentValue(UpgradeType type);
        StatUpgradeConfig GetConfig(UpgradeType type);
        public void TryUpgrade(UpgradeType type, float playerMoney);
    }
}