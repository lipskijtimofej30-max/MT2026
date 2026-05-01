using System;
using Game.Scripts.Service;
using Game.Scripts.UpgradeSystem;

namespace Game.Scripts.CameraPhoto
{
    public class CooldownModule
    {
        private IProgressionService _progressionService;
        public float CurrentTime { get; private set; }
        public bool IsReady =>  CurrentTime <= 0f;

        public CooldownModule(IProgressionService progressionService)
        {
            _progressionService = progressionService;
        }
        
        public void Progress(float deltaTime) => CurrentTime = Math.Max(0f, CurrentTime - deltaTime);
        public void Reset() => CurrentTime = _progressionService.GetCurrentValue(UpgradeType.Cooldown);
    }
}