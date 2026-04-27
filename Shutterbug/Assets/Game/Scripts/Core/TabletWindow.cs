using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Core
{
    public abstract class TabletWindow : MonoBehaviour
    {
        public abstract void ToggleWindow();
        public abstract void SetVisible(bool visible);
        
        protected virtual IEnumerator ShowAnimation(RectTransform windowRoot, CanvasGroup canvasGroup,float showDuration,Ease showEase)
        {
            windowRoot.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            var sequence = DOTween.Sequence();
            sequence.Join(windowRoot.DOScale(1f, showDuration).SetEase(showEase));
            sequence.Join(canvasGroup.DOFade(1f, showDuration).SetEase(showEase));

            yield return sequence.WaitForCompletion();
        }
        
        protected virtual IEnumerator HideAnimation(RectTransform windowRoot, CanvasGroup canvasGroup,float hideDuration,Ease hideEase)
        {
            var sequence = DOTween.Sequence();
            sequence.Join(windowRoot.DOScale(0f, hideDuration).SetEase(hideEase));
            sequence.Join(canvasGroup.DOFade(0f, hideDuration).SetEase(hideEase));

            yield return sequence.WaitForCompletion();
        }
    }
}