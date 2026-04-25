using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class TabletState : IGameState
    {
        public GameMode GameMode => GameMode.Tablet;
        
        private readonly TabletView _tabletView;

        public TabletState(TabletView tabletView)
        {
            _tabletView = tabletView;
        }
        public void Enter()
        {
            _tabletView.Open();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void Exit()
        {
            _tabletView.Close();
        }

        public void Update()
        {
        }
    }
}