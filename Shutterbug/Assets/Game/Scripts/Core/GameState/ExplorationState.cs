using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace Game.Scripts.Core
{
    public class ExplorationState: IGameState
    {
        public GameMode GameMode => GameMode.Exploration;
        
        private PlayerController _playerController;
        
        [Inject]
        private void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }
        
        public void Enter()
        {
            _playerController.ToggleController(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}