namespace Game.Scripts.Module
{
    public interface IAnimatorModule
    {
        void StartAnimationIdle();
        void StartAnimationWalk();
        void StartAnimationAlert();
        void StartAnimationSpecialState();
        void StartAnimationEating();
    }
}