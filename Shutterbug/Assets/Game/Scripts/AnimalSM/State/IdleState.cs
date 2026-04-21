using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class IdleState : IState
    {
        private float _minTimeIdle, _maxTimeIdle, _distToFlee;
        private GameMath _gameMath;
        private NavMeshAgent _agent;
        private Animator _animator;

        public IdleState(GameMath gameMath,NavMeshAgent agent,Animator animator, float distToFlee, float minTime, float maxTime)
        {
            _gameMath = gameMath;
            _agent = agent;
            _animator = animator;
            _distToFlee = distToFlee;
            _minTimeIdle = minTime;
            _maxTimeIdle = maxTime;
        }
        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            float elapsed = 0f;
            float duration = Random.Range(_minTimeIdle, _maxTimeIdle);
            
            _animator.SetInteger("AnimIndex", 0);
            _animator.SetTrigger("Next");
            
            while (elapsed < duration)
            {
                if (_gameMath.DistanceToPlayer(_agent.transform) < _distToFlee)
                    return StateAction.GoToFlee;

                await UniTask.Delay(100, cancellationToken: ct); 
                elapsed += 0.1f;
            }

            if (Random.value < 0.3f)
                return StateAction.Stay;

            return StateAction.GoToWalk;
        }
    }
}
