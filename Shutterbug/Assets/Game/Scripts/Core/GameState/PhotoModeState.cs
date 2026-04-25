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

        [Inject]
        private void Construct(CameraCaptureView cameraCaptureView, CameraCaptureController cameraCaptureController)
        {
            _cameraCaptureView = cameraCaptureView;
            _cameraCaptureController = cameraCaptureController;
        }

        public void Enter()
        {
            _cameraCaptureView.SetUIActive(true);
            _cameraCaptureController.OnEnterPhotoMode();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Exit()
        {
            _cameraCaptureView.SetUIActive(false);
            _cameraCaptureController.OnExitPhotoMode();
        }

        public void Update()
        {
        }
    }
}