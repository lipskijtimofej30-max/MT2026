using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Core
{
    public class GameInputRouter : MonoBehaviour
    {
        private GameStateMachine _modeManager;

        [Inject]
        private void Construct(GameStateMachine stateMachine)
        {
            _modeManager = stateMachine;
        }

        private void Start()
        {
            _modeManager.OnStateChanged += OnModeChange;
        }

        private void OnDestroy()
        {
            _modeManager.OnStateChanged -= OnModeChange;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
                _modeManager.SwitchState(GameMode.Photo);
            if (Input.GetMouseButtonUp(1))
                _modeManager.SwitchState(GameMode.Exploration);

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (_modeManager.CurrentMode == GameMode.Tablet)
                    _modeManager.SwitchState(GameMode.Exploration);
                else
                    _modeManager.SwitchState(GameMode.Tablet);
            }
        }
        
        private void OnModeChange(GameMode oldMode,GameMode newMode)
        {   
            Debug.LogWarning($"Game mode was {oldMode} change to {newMode}");
        }
    }
}