using Game.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Core
{
    public class TabletState : IGameState
    {
        public GameMode GameMode => GameMode.Tablet;
        private TabletView _tabletView;
        
        [Inject]
        private void Construct(TabletView tabletView)
        {
            _tabletView = tabletView;
        }

        public void Enter()
        {
            _tabletView.OnEnterState();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void Exit()
        {
            _tabletView.OnExitState();
        }

        public void Update()
        {
        }
    }
}