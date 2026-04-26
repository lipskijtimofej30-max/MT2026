using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeUpdater : MonoBehaviour
{
    [SerializeField] private RectTransform _target;
    private RectTransform _rect;
    private void OnValidate()
    {
        _rect = GetComponent<RectTransform>();
        if (_target != null)
            _rect.sizeDelta = _target.sizeDelta;
    }
}
