using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using DG.Tweening;

namespace SthGame
{
    public abstract class UIBasePopupController : UIBaseController
    {
        UIBasePopupView view;
        CanvasGroup _canvasGroup;

        protected CanvasGroup canvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = UINode.gameObject.GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                    {
                        _canvasGroup = UINode.gameObject.AddComponent<CanvasGroup>();
                    }
                }
                return _canvasGroup;
            }
        }

        public override void Init()
        {
            base.Init();

            view = UINode as UIBasePopupView;

            if (view.bgBtn != null) view.bgBtn.onClick.AddListener(OnClickClose);
            if (view.closeBtn != null) view.closeBtn.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            DoPopupAnimation();
        }

        private void DoPopupAnimation()
        {
            view.transform.DOScale(0.85f, 0f);
            view.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            canvasGroup.DOFade(0f, 0f);
            canvasGroup.DOFade(1f, 0.3f);
        }
    }
}