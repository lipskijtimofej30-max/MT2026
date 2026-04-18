using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts;
using UnityEngine;

public class StateMachine : IDisposable
{
    private readonly Dictionary<StateAction, IState> _transitionMap;
    private IState _currentState;
    private CancellationTokenSource _cts;
    private bool _isRunning;

    public StateMachine(Dictionary<StateAction, IState> map)
    {
        _transitionMap = map;
    }

    public async UniTask Start(IState initState)
    {
        if (_isRunning)
        {
            Debug.LogWarning("StateMachine already running");
            return;
        }

        _isRunning = true;
        _currentState = initState;

        try
        {
            await RunStateLoop();
        }
        finally
        {
            _isRunning = false;
        }
    }

    private async UniTask RunStateLoop()
    {
        while (_isRunning)
        {
            // Отменяем предыдущую операцию, если она ещё выполняется
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            StateAction nextAction;

            try
            {
                nextAction = await _currentState.OnEnter(_cts.Token);
                Debug.Log($"State {_currentState.GetType().Name} returned {nextAction}");
            }
            catch (OperationCanceledException)
            {
                // Состояние было прервано переходом в новое состояние — продолжаем цикл
                continue;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception in state {_currentState.GetType().Name}: {e}");
                break;
            }

            if (nextAction == StateAction.Stay)
            {
                // Остаёмся в том же состоянии, запускаем его снова (можно добавить небольшую задержку)
                await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
                continue;
            }

            if (_transitionMap.TryGetValue(nextAction, out var nextState))
            {
                _currentState = nextState;
                // Цикл продолжается автоматически
            }
            else
            {
                Debug.LogError($"No transition found for action {nextAction}. Stopping state machine.");
                break;
            }
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Stop()
    {
        _isRunning = false;
        _cts?.Cancel();
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
    }
}
