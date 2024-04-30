using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Timers;

namespace SthGame
{
    public class SingleFloatTips : MonoBehaviour
    {
        public Text contentText;
        public CanvasGroup canvasGroup;

        Tweener moveTweener;
        Tweener fadeTweener;

        UnityAction fadeStartAction;                // 此回调用于触发下一个Tips
        UnityAction<SingleFloatTips> fadeEndAction; // 此回调用于回收Tips

        float moveDistance = 80f;
        float animationDuration = 0.5f;
        float duration = 1.5f;

        //public void 
        public void Reset()
        {
            contentText.text = "";
            this.transform.localPosition = Vector3.zero;
            this.gameObject.SetActive(false);
        }

        public void Show(string content, float inDuration = 1.5f, UnityAction startAction = null, UnityAction<SingleFloatTips> endaction = null)
        {
            contentText.text = content;
            fadeStartAction = startAction;
            fadeEndAction = endaction;
            duration = inDuration;
            this.transform.localPosition = Vector3.down * moveDistance;
            fadeTweener = canvasGroup.DOFade(0f, 0f);
            fadeTweener = canvasGroup.DOFade(1f, animationDuration).SetEase(Ease.OutQuad);
            moveTweener = this.transform.DOLocalMoveY(0, animationDuration).
                SetEase(Ease.OutQuad).
                OnComplete(MoveToEnd);
        }

        void MoveToEnd()
        {
            moveTweener = this.transform.DOLocalMoveY(moveDistance, animationDuration).
                SetEase(Ease.OutQuad).
                OnStart(OnFadeStart).
                OnComplete(OnMoveComplete).
                SetDelay(duration);
            fadeTweener = canvasGroup.DOFade(0, animationDuration).SetEase(Ease.InQuad).SetDelay(duration);
        }

        public void MoveToEndImmediately()
        {
            moveTweener.Kill();
            fadeTweener.Kill();

            moveTweener = this.transform.DOLocalMoveY(moveDistance, animationDuration)
                .SetEase(Ease.OutQuad)
                .OnStart(OnFadeStart)
                .OnComplete(OnMoveComplete);
            
            fadeTweener = canvasGroup.DOFade(0, animationDuration).SetEase(Ease.InQuad);
        }

        void OnFadeStart()
        {
            if (fadeStartAction != null)
            {
                fadeStartAction();
            }
        }

        void OnMoveComplete()
        {
            if (fadeEndAction != null)
            {
                fadeEndAction(this);
            }
        }

        private void OnDestroy()
        {
            if (moveTweener != null) moveTweener.Kill();
            if (fadeTweener != null) fadeTweener.Kill();

            Debug.Log("ondes");
        }
    }
}