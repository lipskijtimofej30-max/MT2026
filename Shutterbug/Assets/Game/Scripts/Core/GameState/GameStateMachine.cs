using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Core
{
    public class GameStateMachine: IInitializable, IDisposable
    {
        private readonly Dictionary<GameMode, IGameState> _states;
        private IGameState _currentState;
        
        public GameMode CurrentMode => _currentState?.GameMode ?? GameMode.Exploration;
        public event Action<GameMode, GameMode> OnStateChanged;

        public GameStateMachine(ExplorationState explorationState, PhotoModeState photoModeState, TabletState tabletState)
        {
            _states = new()
            {
                {GameMode.Exploration, explorationState},
                {GameMode.Photo, photoModeState},
                {GameMode.Tablet, tabletState}
            };
        }
 
        public void Initialize()
        {
            SwitchState(GameMode.Exploration);
        }
        
        public void SwitchState(GameMode newMode)
        {
            if (_states.TryGetValue(newMode, out var newState) == false)
            {
                Debug.LogError($"No state for mode {newMode}");
                return;
            }

            if (_currentState == newState) return;

            var prevMode = _currentState?.GameMode ?? GameMode.Exploration;
            _currentState?.Exit();

            _currentState = newState;
            _currentState.Enter();

            OnStateChanged?.Invoke(prevMode, newMode);
        }
        
        public void Dispose()
        {
            _currentState?.Exit();
        }

    }
}