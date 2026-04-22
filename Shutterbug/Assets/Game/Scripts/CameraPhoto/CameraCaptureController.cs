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
        private QuestController _questController;

        [Inject]
        private void Construct(PhotoEvaluator evaluator, IPhotoProvider provider, QuestController quest, IProgressionService progressionService)
        {
            _evaluator = evaluator;
            _provider = provider;
            _questController = quest;
            _progressionService = progressionService;
        }

        private void Awake()
        {
            _view = GetComponent<CameraCaptureView>();
            _zoomModule = new CameraZoomModule(virtualCamera, _progressionService);
            _cooldownModule = new CooldownModule(_progressionService);
            _view.UpdateTimerDisplay(_cooldownModule.CurrentTime);
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            bool isAiming = Input.GetMouseButton(1);
            _view.SetUIActive(isAiming);

            if (isAiming)
            {
                _zoomModule.UpdateZoom(Input.GetAxis("Mouse ScrollWheel"));
                
                _cooldownModule.Progress(Time.deltaTime);
                _view.UpdateTimerDisplay(_cooldownModule.CurrentTime);

                if (_cooldownModule.IsReady && Input.GetKeyDown(KeyCode.E))
                {
                    ExecuteCapture().Forget();
                }
            }
            else
            {
                _zoomModule.ResetZoom();
            }
        }

        private async UniTaskVoid ExecuteCapture()
        {
            var targets = await cameraCapture.Capture();
            
            ProcessResults(targets);

            // 3. Сброс кулдауна
            _cooldownModule.Reset();
        }

        private void ProcessResults(List<BaseAnimalAI> targets)
        {
            if (targets != null && targets.Count > 0)
            {
                var bestTarget = targets[0];
                var score = _evaluator.CalculateScore(bestTarget, bestTarget.CurrentState, virtualCamera);
                
                if (_questController.CurrentQuest.IsCorrectTarget(bestTarget))
                    Debug.LogWarning("Quest Success!");

                Debug.LogWarning($"Animal type {bestTarget.AnimalType}; current state type {bestTarget.CurrentState};");
                _provider.Score = score;
            }
            else
            {
                _provider.Score = new PhotoScore();
            }
        }
    }
}
