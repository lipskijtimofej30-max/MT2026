using System;
using System.Net.NetworkInformation;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Game.Scripts.Service;
using UnityEngine;
using Zenject;

namespace Game.Scripts.CameraPhoto
{
    [RequireComponent(typeof(CameraCaptureView))]
    public class CameraCaptureController : MonoBehaviour
    {
        [SerializeField] private BaseAnimalAI animalAI;
        [SerializeField] private CameraCapture cameraCapture;
        [SerializeField] private float cooldown = 10f;

        [Header("Zoom settings")] 
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _minFOV = 30f;
        [SerializeField] private float _maxFOV = 90f;

        private CameraCaptureView view;
        private PhotoEvaluator photoEvaluator;
        private IPhotoProvider photoProvider;

        private float currentCooldown;
        private bool isUIActive;
        private bool canCapture;
        private bool isCapturing;
        private CancellationTokenSource cts;

        [Inject]
        private void Construct(PhotoEvaluator photoEvaluator, IPhotoProvider photoProvider)
        {
            this.photoEvaluator = photoEvaluator;
            this.photoProvider = photoProvider;
        }

        private void Awake()
        {
            view = GetComponent<CameraCaptureView>();
        }

        private void Start()
        {
            currentCooldown = cooldown;
            view.UpdateTimerDisplay(currentCooldown);
            cts = new CancellationTokenSource();
            RunInputLoopAsync(cts.Token).Forget();
        }
        
        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        private async UniTaskVoid RunInputLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                bool rightMouseHeld = Input.GetMouseButton(1);
                if (rightMouseHeld != isUIActive)
                {
                    isUIActive = rightMouseHeld;
                    view.SetUIActive(isUIActive);
                }

                if (isUIActive)
                {
                    Zoom();

                    if (currentCooldown > 0f)
                    {
                        currentCooldown -= Time.deltaTime;
                        if (currentCooldown <= 0f)
                            canCapture = true;
                        view.UpdateTimerDisplay(currentCooldown);
                    }

                    if (canCapture && !isCapturing && Input.GetKeyDown(KeyCode.E))
                    {
                        canCapture = false;
                        isCapturing = true;

                        try
                        {
                            await CaptureAndEvaluateAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            // Игнорируем отмену при уничтожении
                        }
                        finally
                        {
                            currentCooldown = cooldown;
                            view.UpdateTimerDisplay(currentCooldown);
                            isCapturing = false;
                        }
                    }
                }
                else
                {
                    canCapture = false;
                    _camera.m_Lens.FieldOfView = Mathf.Lerp(_camera.m_Lens.FieldOfView, 60f, 0.4f);
                }
            }
        }

        private async UniTask CaptureAndEvaluateAsync(CancellationToken token)
        {
            // Ждём захват скриншота
            await cameraCapture.Capture().AttachExternalCancellation(token);

            // Вычисляем очки
            var score = photoEvaluator.CalculateScore(animalAI, typeof(WalkState), _camera);
            Debug.Log($"Score: {score.TotalScore}");

            // Устанавливаем очки в провайдер (фото уже там после Capture)
            photoProvider.Score = score;
        }

        private void Zoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float newFOV = _camera.m_Lens.FieldOfView - scroll * _zoomSpeed;
                newFOV = Mathf.Clamp(newFOV, _minFOV, _maxFOV);
                _camera.m_Lens.FieldOfView = newFOV;
            }
        }
    }
}
