using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts;
using Game.Scripts.Core;
using Game.Scripts.Factory;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private TextureSize _captureSize;
    [SerializeField] private TextureSize _thumbnailSize;
    
    private BaseAnimalAI animalAI;
    
    private IPhotoProvider _photoProvider;
    private AnimalRegistry _animalRegistry;
    private IProgressionService _progressionService;

    [Inject]
    private void Construct(IPhotoProvider photoProvider, AnimalRegistry animalRegistry, 
        IProgressionService progressionService)
    {
        _photoProvider = photoProvider;
        _animalRegistry = animalRegistry;
        _progressionService = progressionService;
    }

    public async UniTask<CapturedData> Capture()
    {
        RenderTexture rt = new RenderTexture(_captureSize.Width, _captureSize.Height, 24);
        _targetCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(_captureSize.Width, _captureSize.Height, TextureFormat.RGB24, false);

        _targetCamera.Render();
        RenderTexture.active = rt;

        screenshot.ReadPixels(new Rect(0, 0, _captureSize.Width, _captureSize.Height), 0, 0);
        screenshot.Apply();

        _targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _photoProvider.Photo = screenshot;
        Texture2D thumbnail = screenshot.ResizeTexture(_thumbnailSize.Width, _thumbnailSize.Height);


        return new CapturedData(GetAnimalInFrame(), thumbnail);
    }

    private List<BaseAnimalAI> GetAnimalInFrame()
    {
        List<BaseAnimalAI> result = new List<BaseAnimalAI>();
        var allAnimals = _animalRegistry.Animals;

        foreach (var animal in allAnimals)
        {
            if (animal == null) continue;
        
            var pos = _targetCamera.WorldToViewportPoint(animal.transform.position);
            bool isInFrustum = pos.z > 0 && pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1;
        
            if (isInFrustum)
            {
                if (CheckLineOfSight(_targetCamera, animal))
                {
                    result.Add(animal);
                }
                else
                {
                    Debug.LogWarning($"Animal {animal.name} FAILED line of sight");
                }
            }
        }
        return result.OrderBy(a => Vector2.Distance(new Vector2(0.5f, 0.5f), _targetCamera.WorldToViewportPoint(a.transform.position))).ToList();
    }

    private bool CheckLineOfSight(Camera camera, BaseAnimalAI animal)
    {
        Vector3 direction = animal.transform.position - camera.transform.position;
        float distance = direction.magnitude;
    
        if (distance > _progressionService.CurrentLevelData.captureDistance)
            return false;
        
        Vector3 targetPoint = animal.transform.position + Vector3.up * 0.5f; 
        Vector3 rayDirection = targetPoint - camera.transform.position;
        var objs = Physics.SphereCastAll(camera.transform.position, 0.5f, rayDirection, distance + 1f);
        for (int i = 0; i < 5; i++)
        {
            var obj = objs[i];
            if (obj.collider.transform.root == animal.transform.root)
                return true;
            else
                Debug.LogWarning($"View blocked by: {obj.collider.name}");
        }    
        return false;
    }
}

public class CapturedData
{
    public List<BaseAnimalAI> animals;
    public Texture2D thumbnail;

    public CapturedData(List<BaseAnimalAI> animals, Texture2D thumbnail)
    {
        this.animals = animals;
        this.thumbnail = thumbnail;
    }
}

[Serializable]
public struct TextureSize
{
    public int Width;
    public int Height;
}
