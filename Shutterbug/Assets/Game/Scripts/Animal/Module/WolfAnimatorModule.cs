using UnityEngine;

namespace Game.Scripts.Module
{
    public class WolfAnimatorModule : IAnimatorModule
    {
        private static string AnimIndex ="AnimIndex";
        private Animator _animator;
        public WolfAnimatorModule(Animator animator)
        {
            _animator = animator;
        }
        public void StartAnimationIdle()
        {
            var idleIndex = Random.Range(0f, 1f);
            _animator.SetFloat(AnimIndex, 0.5f);
            _animator.SetFloat("Idle", idleIndex);
        }

        public void StartAnimationWalk()
        {
            _animator.SetFloat(AnimIndex, 0f);
            _animator.SetFloat("Move", 1f);
        }

        public void StartAnimationAlert()
        {
            _animator.SetFloat(AnimIndex, 1f);
        }

        public void StartAnimationSpecialState()
        {
            _animator.SetFloat(AnimIndex, 0f);
            _animator.SetFloat("Move", 0f);
        }

        public void StartAnimationEating()
        {
            _animator.SetFloat(AnimIndex, 0.75f);
        }

        public void StartAnimationAttack()
        {
            var attackIndex = Random.Range(0f, 1f);
            _animator.SetFloat(AnimIndex, 0.25f);
            _animator.SetFloat("Attack", attackIndex);
        }
    }
}