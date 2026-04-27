using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Module;
using UnityEngine;

namespace Game.Scripts
{
    public class AlertState : IState
    {
        private readonly RabbitAnimatorModule _animatorModule;
        private readonly RabbitLookAt _rabbitLookAt;
        private readonly Func<bool> _conditionMet;
        private readonly float _minTime, _maxTime;
        public AnimalState StateType => AnimalState.Alert;

        public AlertState(RabbitAnimatorModule animatorModule, RabbitLookAt rabbitLookAt, Func<bool> conditionMetToSpecialState, float minTime, float maxTime)
        {
            _animatorModule = animatorModule;
            _conditionMet = conditionMetToSpecialState;
            _minTime = minTime;
            _maxTime = maxTime;
            _rabbitLookAt = rabbitLookAt;
        }

        public async UniTask<StateAction> OnEnter(CancellationToken ct)
        {
            Debug.Log("[AlertState] Зашли в состояние Alert");
            
            // Запускаем поворот головы в фоне и забываем (Forget), чтобы не было предупреждений
            _rabbitLookAt.StartLooking(ct).Forget();
            
            _animatorModule.StartAnimation(RabbitAnimatorModule.IDLE);
    
            // Создаем токен, который сам отменится через случайное время (тайм-аут)
            float waitTime = UnityEngine.Random.Range(_minTime, _maxTime);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfterSlim(TimeSpan.FromSeconds(waitTime));

            StateAction nextAction = StateAction.GoToIdle;

            try
            {
                // Ждем, пока игрок не подойдет близко (ShouldFlee) ИЛИ пока не выйдет время
                await UniTask.WaitUntil(() => _conditionMet(), cancellationToken: timeoutCts.Token);
                
                // Если код дошел сюда без ошибки отмены, значит условие выполнилось до истечения времени!
                nextAction = StateAction.GoToSpecialState; // Убегаем
            }
            catch (OperationCanceledException)
            {
                // Если StateMachine не отменяли, значит сработал наш тайм-аут
                if (!ct.IsCancellationRequested) 
                {
                    nextAction = StateAction.GoToIdle; // Время вышло, успокаиваемся
                }
                else 
                {
                    // Если отменила сама стейт-машина (принудительная остановка)
                    throw;
                }
            }
            finally
            {
                // Гарантированно плавно опускаем голову перед выходом
                await _rabbitLookAt.StopLooking(CancellationToken.None); 
            }

            Debug.Log($"[AlertState] Выходим с действием: {nextAction}");
            return nextAction;
        }

        public float GetStateMultiplier() => 1.4f;
    }
}
