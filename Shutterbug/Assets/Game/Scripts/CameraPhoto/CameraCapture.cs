using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts;
using Game.Scripts.Factory;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private int captureWidth = 1920;
    [SerializeField] private int captureHeight = 1080;
    
    private BaseAnimalAI animalAI;

    private IPhotoProvider _photoProvider;
    private AnimalRegistry _animalRegistry;
    private IProgressionService _progressionService;

    [Inject]
    private void Construct(IPhotoProvider photoProvider, AnimalRegistry animalRegistry, IProgressionService progressionService)
    {
        _photoProvider = photoProvider;
        _animalRegistry = animalRegistry;
        _progressionService = progressionService;
    }

    public async UniTask<List<BaseAnimalAI>> Capture()
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
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _photoProvider.Photo = screenshot;

        return GetAnimalInFrame();
    }

    private List<BaseAnimalAI> GetAnimalInFrame()
    {
        List<BaseAnimalAI> result = new List<BaseAnimalAI>();
        var allAnimals = _animalRegistry.Animals;
        foreach (var animal in allAnimals)
        {
            if (animal == null) continue;
            
            var pos = targetCamera.WorldToViewportPoint(animal.transform.position);
            bool isInFrustum = pos.z > 0 && pos.x > 0 && pos.x < 1 && pos.y >0 && pos.y < 1;
            if (isInFrustum)
            {
                float distance = Vector3.Distance(targetCamera.transform.position, animal.transform.position);
                if (distance > 50f) continue;
                if (CheckLineOfSight(targetCamera, animal))
                {
                    result.Add(animal);
                }
            }
        }
        return result.OrderBy(a => Vector2.Distance(new Vector2(0.5f, 0.5f), targetCamera.WorldToViewportPoint(a.transform.position))).ToList();
    }

    private bool CheckLineOfSight(Camera camera, BaseAnimalAI animal)
    {
        Ray ray = new Ray(camera.transform.position, animal.transform.position - camera.transform.position);
        if (Physics.SphereCast(ray, 4f, out RaycastHit hit, _progressionService.CurrentLevelData.captureDistance))
        {
            return hit.collider.gameObject == animal.gameObject;
        }
        return false;
    }
}
