using Game.Scripts.CameraPhoto;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Core
{
    public class PhotoModeState: IGameState
    {
        public GameMode GameMode => GameMode.Photo;
        
        private CameraCaptureView _cameraCaptureView;
        private CameraCaptureController _cameraCaptureController;
        private PlayerController _playerController;

        [Inject]
        private void Construct(CameraCaptureView cameraCaptureView, CameraCaptureController cameraCaptureController, PlayerController playerController)
        {
            _cameraCaptureView = cameraCaptureView;
            _cameraCaptureController = cameraCaptureController;
            _playerController = playerController;
        }

        public void Enter()
        {
            _cameraCaptureView.SetUIActive(true);
            _cameraCaptureController.OnEnterPhotoMode();
            _playerController.ToggleController(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Exit()
        {
            _cameraCaptureView.SetUIActive(false);
            _playerController.ToggleController(true);
            _cameraCaptureController.OnExitPhotoMode();
        }

        public void Update()
        {
        }
    }
}