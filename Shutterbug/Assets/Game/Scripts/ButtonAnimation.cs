using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private Button _button;

    [Header("Animation Settings")] 
    [SerializeField] private float _duration = 0.15f;
    [SerializeField] private float _valueAnimation = 0.8f;
    [SerializeField] private Ease _riseEase = Ease.InBack;
    [SerializeField] private Ease _landEase = Ease.OutBack;
    
    void Start()
    {
        _button ??= GetComponent<Button>();
        _button.onClick.AddListener(() => ClickWithAnim());
    }

    private void ClickWithAnim()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(_valueAnimation, _duration).SetEase(_riseEase))
            .Append(transform.DOScale(1f, _duration).SetEase(_landEase));
    }
}
