using Cinemachine;
using UnityEngine;

namespace Game.Scripts.CameraPhoto
{
    public class CameraZoomModule
    {
        private readonly CinemachineVirtualCamera _virtualCamera;

        public CameraZoomModule(CinemachineVirtualCamera virtualCamera)
        {
            _virtualCamera = virtualCamera;
        }

        public void UpdateZoom(float scroll)
        {
            float newFOV = _virtualCamera.m_Lens.FieldOfView - scroll * 10f;
            newFOV = Mathf.Clamp(newFOV, 30f, 60f);
            _virtualCamera.m_Lens.FieldOfView = newFOV;
        }

        public void ResetZoom(float lerpSpeed = 0.4f)
        {
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, 60f, lerpSpeed);
        }
    }
}