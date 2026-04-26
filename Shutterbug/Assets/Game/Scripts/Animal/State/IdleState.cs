using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class IdleState : IState
    {
        private float _minTimeIdle, _maxTimeIdle;
        private RabbitAnimatorModule _animatorModule;
        private Func<bool> _conditionMet;
        public AnimalState StateType => AnimalState.Idle;


        public IdleState(RabbitAnimatorModule animatorModule, Func<bool> conditionMetToSpecialState,float minTime, float maxTime)
        {
            _animatorModule = animatorModule;
            _conditionMet = conditionMetToSpecialState;
            _minTimeIdle = minTime;
            _maxTimeIdle = maxTime;
        }
        
        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            float elapsed = 0f;
            float duration = Random.Range(_minTimeIdle, _maxTimeIdle);
            
            _animatorModule.StartAnimation(RabbitAnimatorModule.IDLE);
            while (elapsed < duration)
            {
                if (_conditionMet())
                    return StateAction.GoToSpecialState;

                await UniTask.Delay(100, cancellationToken: ct); 
                elapsed += 0.1f;
            }

            if (Random.value < 0.3f)
                return StateAction.Stay;

            return StateAction.GoToWalk;
        }
        
        public float GetStateMultiplier() => 1f; 

    }
}
