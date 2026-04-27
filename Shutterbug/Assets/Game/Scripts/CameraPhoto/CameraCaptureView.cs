using System;
using Game.Scripts;
using Game.Scripts.Quest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraCaptureView : MonoBehaviour
{
    [SerializeField] private Image cameraUI;
    [SerializeField] private TMP_Text infoText;
    
    private CameraCapture _cameraCapture;
    private int lastDisplayedTime = -1;
    
    private void Start()
    {
        cameraUI.gameObject.SetActive(false);
    }

    public void Init(CameraCapture cameraCapture)
    {
        _cameraCapture = cameraCapture;
    }

    public void SetUIActive(bool active)
    {
        if (cameraUI != null)
            cameraUI.gameObject.SetActive(active); 
    }

    public void UpdateTimerDisplay(float cooldown)
    {
        if (infoText == null) return;
        int displayValue = Mathf.CeilToInt(cooldown);
        if (displayValue != lastDisplayedTime)
        {
            lastDisplayedTime = displayValue;
            infoText.text = $"{displayValue.ToString()}";
        }
    }
}
