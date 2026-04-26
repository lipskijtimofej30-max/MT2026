using Game.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Core
{
    public class TabletState : IGameState
    {
        public GameMode GameMode => GameMode.Tablet;
        private TabletView _tabletView;
        private PlayerController _playerController;
        
        [Inject]
        private void Construct(TabletView tabletView, PlayerController playerController)
        {
            _tabletView = tabletView;
            _playerController = playerController;
        }

        public void Enter()
        {
            _tabletView.OnEnterState();
            _playerController.ToggleController(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void Exit()
        {
            _playerController.ToggleController(true);
            _tabletView.OnExitState();
        }

        public void Update()
        {
        }
    }
}