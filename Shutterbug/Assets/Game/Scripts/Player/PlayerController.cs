using System;
using Cinemachine;
using DG.Tweening;
using Game.Signals;
using UnityEngine;
using Zenject;

namespace Game.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        private CharacterController controller;
        private Transform cameraTransform;
        
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.2f;
        
        [Header("Jump Settings")]
        [SerializeField] private bool canJump = true;
        
        [Header("Camera Shake Settings")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeStrength = 1f;
        [SerializeField] private int shakeVibrato = 10;
        
        [Header("Head Bobbing Settings")]
        [SerializeField] private bool useHeadBob = true;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private float bobFrequency = 5f;
        [SerializeField] private float bobAmplitude = 0.05f;
        
        [Header("Camera Tilt Settings")]
        [SerializeField] private float tiltAmount = 2f;
        [SerializeField] private float tiltSpeed = 5f;
        
        [Header("Crouch Settings")]
        [SerializeField] private float crouchSpeed = 4f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 10f;
        
        private SignalBus _signalBus;
        private CinemachineBasicMultiChannelPerlin _cinemachineNoise;
        
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private Vector3 _standingCenter = new Vector3(0, 0, 0);
        private Vector3 _crouchCenter = new Vector3(0, -0.5f, 0);
        
        private float _currentSpeed;
        private float _targetRotation;
        private float _currentTilt = 0f;
        private float _rotationVelocity;
        private float _defaultCameraY;
        private float _bobTimer = 0f;
        private float _defaultY;
        
        private bool _isCrouched;
        private bool _isGrounded;

        public bool IsCrouched { get => _isCrouched; set => _isCrouched = value; }
        public bool CanJump { get => canJump; set => canJump = value; }
        
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            _currentSpeed = moveSpeed;
            
            _standingCenter = controller.center;
            _crouchCenter = new Vector3(0, (crouchHeight - standingHeight) / 2f, 0);
            _defaultCameraY = cameraHolder.localPosition.y;
            
            if (Camera.main != null) 
                cameraTransform = Camera.main.transform;
            else 
                Debug.LogError("Main camera not found in scene!");
            
            if (cameraHolder != null)
                _defaultY = cameraHolder.localPosition.y;
            
            if (virtualCamera != null)
            {
                _cinemachineNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if (_cinemachineNoise == null)
                    Debug.LogWarning("CinemachineBasicMultiChannelPerlin not found on virtual camera. Camera shake will use DoTween fallback.");
            }
            else
            {
                Debug.LogError("Cinemachine Virtual Camera is not assigned in the inspector!");
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            HandleCrouch();
            HandleMovementInput();
            HandleGravityAndJump();
            ApplyFinalMovement();
            HandleCameraTilt();
            HandleHeadBob();
        }

        private void HandleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                _currentSpeed = crouchSpeed;
                _isCrouched = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                _currentSpeed = moveSpeed;
                _isCrouched = false;
            }

            float targetHeight = _isCrouched ? crouchHeight : standingHeight;
            Vector3 targetCenter = _isCrouched ? _crouchCenter : _standingCenter;

            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.center = Vector3.Lerp(controller.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);
            
            float eyeLevelMultiplier = 0.9f; 
            float targetCameraY = controller.height * eyeLevelMultiplier + controller.center.y - (controller.height / 2f);
            
            Vector3 newCameraPos = cameraHolder.localPosition;
            newCameraPos.y = Mathf.Lerp(cameraHolder.localPosition.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
            cameraHolder.localPosition = newCameraPos;

            _signalBus.Fire(new PlayerCrouchedSignal(_isCrouched));
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
    
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            
            Vector3 desiredMoveDirection = (camForward * vertical + camRight * horizontal).normalized;
    
            _moveDirection = desiredMoveDirection;
        }

        private void HandleGravityAndJump()
        {
            _isGrounded = controller.isGrounded;
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f; // Прижимаем к земле
            }

            // Обработка прыжка (клавиша Space по умолчанию)
            if (canJump && Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                ShakeCamera(0.15f, 0.5f); // небольшая тряска при прыжке
            }

            // Гравитация
            _velocity.y += gravity * Time.deltaTime;
        }

        private void ApplyFinalMovement()
        {
            Vector3 horizontalMovement = _moveDirection * (_currentSpeed * Time.deltaTime);
            controller.Move(horizontalMovement);
            controller.Move(_velocity * Time.deltaTime);
        }
        
        private void HandleCameraTilt()
        {
            float sidewaysInput = Input.GetAxisRaw("Horizontal");
            
            float targetTilt = -sidewaysInput * tiltAmount;
            
            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
            
            cameraHolder.localRotation = Quaternion.Euler(
                cameraHolder.localRotation.eulerAngles.x,
                cameraHolder.localRotation.eulerAngles.y,
                _currentTilt
            );
        }

        private void HandleHeadBob()
        {
            if (!_isGrounded) return;

            float inputMag = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
            
            if (inputMag > 0.1f)
            {
                _bobTimer += Time.deltaTime * bobFrequency;
                float newY = cameraHolder.localPosition.y + Mathf.Sin(_bobTimer) * bobAmplitude;
                cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, newY, cameraHolder.localPosition.z);
            }
        }

        /// <summary>
        /// Вызывает тряску камеры.
        /// </summary>
        public void ShakeCamera(float duration, float strength)
        {
            if (virtualCamera == null) return;
            
            // Используем шум Cinemachine если он есть
            if (_cinemachineNoise != null)
            {
                _cinemachineNoise.m_AmplitudeGain = strength;
                DOTween.To(() => _cinemachineNoise.m_AmplitudeGain, x => _cinemachineNoise.m_AmplitudeGain = x, 0f, duration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => _cinemachineNoise.m_AmplitudeGain = 0f);
            }
            else
            {
                virtualCamera.transform.DOShakePosition(duration, strength, shakeVibrato).SetUpdate(true);
            }
        }
        
        public void ToggleController(bool toggle)
        { 
            enabled = toggle;
            virtualCamera.enabled = toggle;
    
            if (!toggle)
            {
                _moveDirection = Vector3.zero;
                _velocity = Vector3.zero;
                if (cameraHolder != null) 
                    cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, _defaultY, cameraHolder.localPosition.z);
            }
        }

        public void Toggle(bool toggle)
        {
            enabled = toggle;
        }
    }
}
