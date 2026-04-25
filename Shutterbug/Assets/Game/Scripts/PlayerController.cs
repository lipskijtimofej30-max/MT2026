using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

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
        public bool CanJump { get => canJump; set => canJump = value; }
        
        [Header("Camera Shake Settings")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeStrength = 1f;
        [SerializeField] private int shakeVibrato = 10;
        
        private Vector3 moveDirection;
        private Vector3 velocity;
        private float currentSpeed;
        private float targetRotation;
        private float rotationVelocity;
        private bool isGrounded;
        private CinemachineBasicMultiChannelPerlin cinemachineNoise;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            currentSpeed = moveSpeed;
            
            if (Camera.main != null) 
                cameraTransform = Camera.main.transform;
            else 
                Debug.LogError("Main camera not found in scene!");
            
            if (virtualCamera != null)
            {
                cinemachineNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if (cinemachineNoise == null)
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
            HandleMovementInput();
            HandleGravityAndJump();
            ApplyFinalMovement();
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
    
            moveDirection = desiredMoveDirection;
        }

        private void HandleGravityAndJump()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Прижимаем к земле
            }

            // Обработка прыжка (клавиша Space по умолчанию)
            if (canJump && Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                ShakeCamera(0.15f, 0.5f); // небольшая тряска при прыжке
            }

            // Гравитация
            velocity.y += gravity * Time.deltaTime;
        }

        private void ApplyFinalMovement()
        {

            Vector3 horizontalMovement = moveDirection * (currentSpeed * Time.deltaTime);
            controller.Move(horizontalMovement);
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Вызывает тряску камеры.
        /// </summary>
        public void ShakeCamera(float duration, float strength)
        {
            if (virtualCamera == null) return;
            
            // Используем шум Cinemachine если он есть
            if (cinemachineNoise != null)
            {
                cinemachineNoise.m_AmplitudeGain = strength;
                DOTween.To(() => cinemachineNoise.m_AmplitudeGain, x => cinemachineNoise.m_AmplitudeGain = x, 0f, duration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => cinemachineNoise.m_AmplitudeGain = 0f);
            }
            else
            {
                // Fallback: трясём позицию камеры через DoTween
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
