using UnityEngine;

namespace Game.Scripts.Module
{
    public class WolfAnimatorModule : IAnimatorModule
    {
        private Animator _animator;
        public WolfAnimatorModule(Animator animator)
        {
            _animator = animator;
        }
        public void StartAnimationIdle()
        {
            _animator.SetFloat("Vert", 0);
        }

        public void StartAnimationWalk()
        {
            _animator.SetFloat("Vert", 1);
            _animator.SetFloat("State", 0);
        }

        public void StartAnimationAlert()
        {
            StartAnimationIdle();
        }

        public void StartAnimationSpecialState()
        {
            _animator.SetFloat("Vert", 1);
            _animator.SetFloat("State", 1);
        }

        public void StartAnimationEating()
        {
            StartAnimationIdle();
        }
    }
}