using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCardPopUp : MonoBehaviour
{
    [SerializeField] private Button _popUpButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Transform _popUpContainer;

    private void Start()
    {
        _popUpContainer.gameObject.SetActive(false);
        _popUpButton.onClick.AddListener(() => _popUpContainer.gameObject.SetActive(true));
        _cancelButton.onClick.AddListener(() => _popUpContainer.gameObject.SetActive(false));
    }
}
