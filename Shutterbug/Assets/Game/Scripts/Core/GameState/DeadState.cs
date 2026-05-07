namespace Game.Scripts.Core
{
    public class DeadState : IGameState
    {
        private readonly CameraCaptureView _view;
        public GameMode GameMode => GameMode.Dead;

        public DeadState(CameraCaptureView view)
        {
            _view = view;
        }
        public void Enter()
        {
            _view.SetUIActive(false);
        }

        public void Exit()
        {
            //_view.SetUIActive(false);
        }

        public void Update()
        {
        }
    }
}