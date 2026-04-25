using Game.Scripts.CameraPhoto;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class PhotoModeState: IGameState
    {
        public GameMode GameMode => GameMode.Photo;
        
        private readonly CameraCaptureView _cameraCaptureView;
        private readonly CameraCaptureController _cameraCaptureController;

        public PhotoModeState(CameraCaptureView cameraCaptureView, CameraCaptureController cameraCaptureController)
        {
            _cameraCaptureView = cameraCaptureView;
            _cameraCaptureController = cameraCaptureController;
        }
        public void Enter()
        {
            _cameraCaptureView.SetUIActive(true);
            _cameraCaptureController.OnEnterPhotoMode();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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