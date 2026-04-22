using Cinemachine;
using Game.Scripts.Service;
using UnityEngine;

namespace Game.Scripts.CameraPhoto
{
    public class CameraZoomModule
    {
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly IProgressionService _progressionService;

        public CameraZoomModule(CinemachineVirtualCamera virtualCamera, IProgressionService progressionService)
        {
            _virtualCamera = virtualCamera;
            _progressionService = progressionService;
        }

        public void UpdateZoom(float scroll)
        {
            float newFOV = _virtualCamera.m_Lens.FieldOfView - scroll * _progressionService.CurrentLevelData.zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, _progressionService.CurrentLevelData.minFOV, _progressionService.CurrentLevelData.maxFOV);
            _virtualCamera.m_Lens.FieldOfView = newFOV;
        }

        public void ResetZoom(float lerpSpeed = 0.4f)
        {
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, 60f, lerpSpeed);
        }
    }
}