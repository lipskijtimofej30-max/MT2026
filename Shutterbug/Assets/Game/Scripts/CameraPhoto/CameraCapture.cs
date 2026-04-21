using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.CameraPhoto;
using Game.Scripts.Service;
using TMPro.Examples;
using UnityEngine;
using Zenject;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private int captureWidth = 1920;
    [SerializeField] private int captureHeight = 1080;
    
    private IPhotoProvider _photoProvider;

    [Inject]
    private void Construct(IPhotoProvider photoProvider)
    {
        _photoProvider = photoProvider;
    }

    public async UniTask Capture()
    {
        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        targetCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        targetCamera.Render();
        RenderTexture.active = rt;

        screenshot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenshot.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _photoProvider.Photo = screenshot;
    }
}
