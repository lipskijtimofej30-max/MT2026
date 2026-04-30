using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Scripts.Module;
using UnityEngine;

namespace Game.Scripts
{
    public class AlertState : IState
    {
        private readonly RabbitAnimatorModule _animatorModule;
        private readonly RabbitLookAt _rabbitLookAt;
        private readonly Func<bool> _conditionMet;
        private readonly RabbitConfig _config;
        public AnimalState StateType => AnimalState.Alert;

        public AlertState(RabbitAnimatorModule animatorModule, RabbitLookAt rabbitLookAt,
            Func<bool> conditionMetToSpecialState, RabbitConfig config)
        {
            _animatorModule = animatorModule;
            _conditionMet = conditionMetToSpecialState;
            _rabbitLookAt = rabbitLookAt;
            _config = config;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            Debug.Log("[AlertState] Зашли в состояние Alert");
            
            _rabbitLookAt.StartLooking(ct).Forget();
            
            _animatorModule.StartAnimation(RabbitAnimatorModule.IDLE);
    
            float waitTime = UnityEngine.Random.Range(_config.AlertTime.Min, _config.AlertTime.Max);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfterSlim(TimeSpan.FromSeconds(waitTime));

            StateAction nextAction = StateAction.GoToIdle;

            try
            {
                await UniTask.WaitUntil(() => _conditionMet(), cancellationToken: timeoutCts.Token);
                
                nextAction = StateAction.GoToSpecialState; // Убегаем
            }
            catch (OperationCanceledException)
            {
                if (!ct.IsCancellationRequested) 
                {
                    nextAction = StateAction.GoToIdle; // Время вышло, успокаиваемся
                }
                else 
                {
                    throw;
                }
            }
            finally
            {
                await _rabbitLookAt.StopLooking(CancellationToken.None); 
            }

            Debug.Log($"[AlertState] Выходим с действием: {nextAction}");
            return nextAction;
        }

        public float GetStateMultiplier() => 1.4f;
    }
}
