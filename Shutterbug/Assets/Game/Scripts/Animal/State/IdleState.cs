using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class IdleState : IState
    {
        private float _minTimeIdle, _maxTimeIdle;
        private RabbitAnimatorModule _animatorModule;
        private Func<bool> _conditionMetToAlert;
        private Func<bool> _conditionMetToSpecialState;
        public AnimalState StateType => AnimalState.Idle;

        /// <summary>
        /// Передавать метод для перехода в состоние Alert
        /// </summary>
        public IdleState(RabbitAnimatorModule animatorModule, Func<bool> conditionMetToAlert, Func<bool> conditionMetToSpecialState,AnimalConfig config)
        {
            _animatorModule = animatorModule;
            _conditionMetToAlert = conditionMetToAlert;
            _conditionMetToSpecialState = conditionMetToSpecialState;
            _minTimeIdle = config.IdleTime.Min;
            _maxTimeIdle = config.IdleTime.Max;
        }
        
        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            float elapsed = 0f;
            float duration = Random.Range(_minTimeIdle, _maxTimeIdle);
            
            _animatorModule.StartAnimation(RabbitAnimatorModule.IDLE);
            while (elapsed < duration)
            {
                if (_conditionMetToAlert())
                    return StateAction.GoToAlert;
                if(_conditionMetToSpecialState())
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
