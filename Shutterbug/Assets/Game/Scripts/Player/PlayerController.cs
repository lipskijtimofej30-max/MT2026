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
        [SerializeField] private float crouchSpeed = 4f;
        
        [Header("Jump Settings")]
        [SerializeField] private bool canJump = true;
        public bool CanJump { get => canJump; set => canJump = value; }
        
        [Header("Camera Shake Settings")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeStrength = 1f;
        [SerializeField] private int shakeVibrato = 10;
        
        private SignalBus _signalBus;
        
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private float _currentSpeed;
        private float _targetRotation;
        private float _rotationVelocity;
        private bool _isGrounded;
        private float _scaleY;
        private CinemachineBasicMultiChannelPerlin _cinemachineNoise;
        private bool _isCrouched;
        public bool IsCrouched { get => _isCrouched; set => _isCrouched = value; }

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }


        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            _currentSpeed = moveSpeed;
            _scaleY = gameObject.transform.localScale.y;
            
            if (Camera.main != null) 
                cameraTransform = Camera.main.transform;
            else 
                Debug.LogError("Main camera not found in scene!");
            
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
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            HandleCrouch();
            HandleMovementInput();
            HandleGravityAndJump();
            ApplyFinalMovement();
        }

        private void HandleCrouch()
        {
            try
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    _isCrouched = true;
                    _currentSpeed = crouchSpeed;
                    gameObject.transform.DOScaleY(_scaleY/2, 0.2f);
                }
                else
                {
                    _isCrouched = false;
                    _currentSpeed = moveSpeed;
                    gameObject.transform.DOScaleY(_scaleY, 0.2f);
                }
            }
            finally
            {
                _signalBus.Fire(new PlayerCrouchedSignal(_isCrouched));
            }
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
            virtualCamera.enabled = toggle;
            enabled = toggle;
        }
    }
}
