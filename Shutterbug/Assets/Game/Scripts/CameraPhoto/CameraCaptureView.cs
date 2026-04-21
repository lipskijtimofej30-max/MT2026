using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraCaptureView : MonoBehaviour
{
    [SerializeField] private Image cameraUI;
    [SerializeField] private TMP_Text timerText;

    private int lastDisplayedTime = -1;

    private void Start()
    {
        cameraUI.gameObject.SetActive(false); 
    }

    public void SetUIActive(bool active)
    {
        if (cameraUI != null)
            cameraUI.gameObject.SetActive(active);
    }

    public void UpdateTimerDisplay(float cooldown)
    {
        if (timerText == null) return;
        int displayValue = Mathf.CeilToInt(cooldown);
        if (displayValue != lastDisplayedTime)
        {
            lastDisplayedTime = displayValue;
            timerText.text = displayValue.ToString();
        }
    }
}
