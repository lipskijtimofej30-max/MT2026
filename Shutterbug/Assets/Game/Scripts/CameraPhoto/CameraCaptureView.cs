using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraCaptureView : MonoBehaviour
{
    [SerializeField] private CameraCapture _cameraCapture;
    [SerializeField] private Image _cameraUI;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private float _cooldown = 10f;

    private float _currentCooldown;
    private bool _isUIActive;
    private bool _canCapture;
    private bool _isCapturing; // защита от повторного захвата во время асинхронной операции

    private int _lastDisplayedTime = -1; // для оптимизации обновления текста

    private void Start()
    {
        // Базовая валидация
        if (_cameraCapture == null) Debug.LogError($"{nameof(_cameraCapture)} is not assigned!");
        if (_cameraUI == null) Debug.LogError($"{nameof(_cameraUI)} is not assigned!");
        if (_timerText == null) Debug.LogError($"{nameof(_timerText)} is not assigned!");

        _cameraUI?.gameObject.SetActive(false);
        _currentCooldown = _cooldown;
        UpdateTimerDisplay();
        RunLoopAsync(destroyCancellationToken).Forget();
    }

    private async UniTaskVoid RunLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            bool rightMouseHeld = Input.GetMouseButton(1);
            if (rightMouseHeld != _isUIActive)
            {
                _isUIActive = rightMouseHeld;
                _cameraUI.gameObject.SetActive(_isUIActive);
            }

            if (_isUIActive)
            {
                // Обновление кулдауна
                if (_currentCooldown > 0f)
                {
                    _currentCooldown -= Time.deltaTime;
                    if (_currentCooldown <= 0f)
                        _canCapture = true;
                    UpdateTimerDisplay();
                }

                // Попытка захвата
                if (_canCapture && !_isCapturing && Input.GetKeyDown(KeyCode.E))
                {
                    _canCapture = false;
                    _isCapturing = true;
                    // Запускаем захват асинхронно, не блокируя цикл
                    CaptureAsync(token).Forget();
                }
            }
            else
            {
                // UI скрыт – сбрасываем возможность захвата (по желанию можно оставить кулдаун)
                _canCapture = false;
                // Если нужно, чтобы кулдаун продолжался в фоне – закомментируйте строку ниже
                // _currentCooldown = _cooldown;
                // UpdateTimerDisplay();
            }
        }
    }

    private async UniTaskVoid CaptureAsync(CancellationToken token)
    {
        try
        {
            await _cameraCapture.Capture().AttachExternalCancellation(token);
        }
        catch (System.OperationCanceledException)
        {
            // Обработка отмены при уничтожении объекта
        }
        finally
        {
            // Сброс кулдауна после завершения захвата
            _currentCooldown = _cooldown;
            UpdateTimerDisplay();
            _isCapturing = false;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (_timerText == null) return;
        int displayValue = Mathf.CeilToInt(_currentCooldown);
        if (displayValue != _lastDisplayedTime)
        {
            _lastDisplayedTime = displayValue;
            _timerText.text = displayValue.ToString();
        }
    }
}