using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Game.Scripts.Quest;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto
{
    [RequireComponent(typeof(CameraCaptureView))] 
    public class CameraCaptureController : MonoBehaviour
    {
        [SerializeField] private CameraCapture cameraCapture;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private CameraCaptureView _view;
        private CameraZoomModule _zoomModule;
        private CooldownModule _cooldownModule;
        
        private PhotoEvaluator _evaluator;
        private IProgressionService _progressionService;
        private IPhotoProvider _provider;
        private IAnimalInPhotoProvider _animalInPhotoProvider;

        [Inject]
        private void Construct(PhotoEvaluator evaluator, IPhotoProvider provider, IProgressionService progressionService, IAnimalInPhotoProvider animalInPhotoProvider)
        {
            _evaluator = evaluator;
            _provider = provider;
            _progressionService = progressionService;
            _animalInPhotoProvider = animalInPhotoProvider;
        }

        private void Awake()
        {
            _view = GetComponent<CameraCaptureView>();
            
            _zoomModule = new CameraZoomModule(virtualCamera, _progressionService);
            _cooldownModule = new CooldownModule(_progressionService);
            
            _view.UpdateTimerDisplay(_cooldownModule.CurrentTime);
            _cooldownModule.Reset();
        }

        public void OnEnterPhotoMode()
        {
            _cooldownModule.Reset();
            _zoomModule.ResetZoom();
            _view.UpdateTimerDisplay(_cooldownModule.CurrentTime);
            enabled = true; 
        }

        public void OnExitPhotoMode()
        {
            enabled = false;
            _zoomModule.ResetZoom(); 
        }

        private void Update()
        {
            _zoomModule.UpdateZoom(Input.GetAxis("Mouse ScrollWheel"));
            
            _cooldownModule.Progress(Time.deltaTime);
            _view.UpdateTimerDisplay(_cooldownModule.CurrentTime);

            if (_cooldownModule.IsReady && Input.GetKeyDown(KeyCode.E))
            {
                ExecuteCapture().Forget();
            }
        }

        private async UniTaskVoid ExecuteCapture()
        {
            var targets = await cameraCapture.Capture();
            
            ProcessResults(targets);

            _cooldownModule.Reset();
        }

        private void ProcessResults(List<BaseAnimalAI> targets)
        {
            if (targets != null && targets.Count > 0)
            {
                var bestTarget = targets[0];
                var score = _evaluator.CalculateScore(bestTarget, virtualCamera);
                _animalInPhotoProvider.LastPhotoData = new CapturedPhotoData
                {
                    AnimalType =  bestTarget.AnimalType,
                    AnimalState = bestTarget.CurrentState,
                };
                Debug.LogWarning($"Animal type {bestTarget.AnimalType}; current state type {bestTarget.CurrentState}; Distance to animal {Vector3.Distance(bestTarget.transform.position, virtualCamera.transform.position)} ");
                _provider.Score = score;
            }
            else
            {
                _provider.Score = new PhotoScore();
            }
        }
    }
}
