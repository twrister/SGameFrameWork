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
            canvasGroup.DOFade(0f, 0f);
            canvasGroup.DOFade(1f, animationDuration).SetEase(Ease.OutQuad);
            this.transform.DOLocalMoveY(0, animationDuration).SetEase(Ease.InOutQuad).OnComplete(OnMoveComplete);
        }

        void OnMoveComplete()
        {
            this.transform.DOLocalMoveY(moveDistance, animationDuration).
                SetEase(Ease.InOutQuad).
                OnStart(OnFadeStart).
                OnComplete(OnFadeComplete).
                SetDelay(duration);
            canvasGroup.DOFade(0, animationDuration).SetEase(Ease.InQuad).SetDelay(duration);
        }

        void OnFadeStart()
        {
            if (fadeStartAction != null)
            {
                fadeStartAction();
            }
        }

        void OnFadeComplete()
        {
            if (fadeEndAction != null)
            {
                fadeEndAction(this);
            }
        }
    }
}