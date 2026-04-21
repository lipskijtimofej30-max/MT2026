using System;

namespace Game.Scripts.CameraPhoto
{
    public class CooldownModule
    {
        public float CurrentTime { get; private set; }
        public bool IsReady =>  CurrentTime <= 0f;
        
        public void Progress(float deltaTime) => CurrentTime = Math.Max(0f, CurrentTime - deltaTime);
        public void Reset(float duration) => CurrentTime = duration;
    }
}