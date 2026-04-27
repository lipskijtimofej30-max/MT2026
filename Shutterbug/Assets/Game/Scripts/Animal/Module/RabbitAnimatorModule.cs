using UnityEngine;

namespace Game.Scripts.Module
{
    public class RabbitAnimatorModule
    {
        public const int IDLE = 0;
        public const int WALKORFLEE = 1;
        
        private Animator _animator;

        public RabbitAnimatorModule(Animator animator)
        {
            _animator = animator;
        }

        public void StartAnimation(int index = 0)
        {
            _animator.SetInteger("AnimIndex", index);
            NextAnimation();
        }
        
        private void NextAnimation() => _animator.SetTrigger("Next");
    }
}
