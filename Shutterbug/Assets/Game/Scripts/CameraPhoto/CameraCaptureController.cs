using Cinemachine;
using Cysharp.Threading.Tasks;
using Game.Scripts.CameraPhoto.PhotoAlbum;
using Game.Scripts.Service;
using Game.Service.Currency;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto
{
    [RequireComponent(typeof(CameraCaptureView))] 
    public class CameraCaptureController : MonoBehaviour
    {
        [SerializeField] private CameraCapture cameraCapture;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private Camera _mainCamera;
        private LayerMask _layerMask;
        
        private CameraCaptureView _view;
        private CameraZoomModule _zoomModule;
        private CooldownModule _cooldownModule;
        
        private PhotoEvaluator _evaluator;
        private IProgressionService _progressionService;
        private IPhotoProvider _provider;
        private IAnimalInPhotoProvider _animalInPhotoProvider;
        private ICurrencyService _currencyService;
        private PhotoService _photoService;

        [Inject]
        private void Construct(PhotoEvaluator evaluator, IPhotoProvider provider, IProgressionService progressionService,
            IAnimalInPhotoProvider animalInPhotoProvider, PhotoService photoService, ICurrencyService currencyService)
        {
            _evaluator = evaluator;
            _provider = provider;
            _progressionService = progressionService;
            _animalInPhotoProvider = animalInPhotoProvider;
            _photoService = photoService;
            _currencyService = currencyService;
        }

        private void Awake()
        {
            _view = GetComponent<CameraCaptureView>();
            _mainCamera = Camera.main;
            _layerMask = LayerMask.GetMask("Animal");
            
            _zoomModule = new CameraZoomModule(virtualCamera, _progressionService);
            _cooldownModule = new CooldownModule(_progressionService);
                
            _view.Init(cameraCapture);
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
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, _progressionService.CurrentLevelData.captureDistance, _layerMask))
            {
                if (hit.collider.TryGetComponent(out BaseAnimalBrain animal))
                {
                    _view.SetInfoText($"{animal.AnimalType},  {animal.CurrentState}");
                }
            }
            else
            {
                _view.SetInfoText("");
            }
            
            if (_cooldownModule.IsReady && Input.GetKeyDown(KeyCode.E))
            {
                ExecuteCapture().Forget();
            }

        }

        private async UniTaskVoid ExecuteCapture()
        {
            var data = await cameraCapture.Capture();
            ProcessResults(data);

            _cooldownModule.Reset();
        }

        private void ProcessResults(CapturedData data)
        {
            if (data.animals != null && data.animals.Count > 0)
            {
                var bestTarget = data.animals[0];
                var score = _evaluator.CalculateScore(bestTarget, virtualCamera);
                _animalInPhotoProvider.LastPhotoData = new CapturedPhotoData(bestTarget.AnimalType, bestTarget.StateMachine.CurrentState);
                Debug.LogWarning($"Animal type {bestTarget.AnimalType}; current state type {bestTarget.CurrentState}; Distance to animal {Vector3.Distance(bestTarget.transform.position, virtualCamera.transform.position)} ");
                _provider.Score = score;
                _photoService.AddPhoto(new PhotoRecord(data.thumbnail, bestTarget.AnimalType, bestTarget.StateMachine.CurrentState, score,
                    System.DateTime.Now.ToString("HH:mm:ss"), false));
            }
            else
            {
                _provider.Score = new PhotoScore();
            }
        }
    }
}
