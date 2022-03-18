using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace SthGame
{
    public class HorseLampTipsController : UIBaseController
    {
        HorseLampTipsView m_View;

        float m_Speed = 150f;
        bool m_IsPlaying = false;

        protected override string GetResourcePath()
        {
            return "Prefabs/HorseLampTipsView";
        }

        public override void Init()
        {
            base.Init();

            m_View = UINode as HorseLampTipsView;

            m_View.m_CanvasGroup.alpha = 0f;
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();
        }

        public void ShowTips(string content)
        {
            if (m_IsPlaying) return;
            if (string.IsNullOrEmpty(content)) return;

            m_IsPlaying = true;
            m_View.m_Content.text = content;

            m_View.m_CanvasGroup.DOFade(1, 0.3f).onComplete = () => {
                m_View.m_Content.transform.localPosition = Vector3.right * 250f;

                float dis = m_View.m_Content.rectTransform.sizeDelta.x + 500f;
                float moveDis = m_View.m_Content.transform.localPosition.x - dis;

                m_View.m_Content.transform.DOLocalMoveX(moveDis, Mathf.Abs(dis) / m_Speed).SetEase(Ease.Linear).onComplete = () =>
                {
                    m_View.m_CanvasGroup.DOFade(0, 0.3f).onComplete = () => {
                        m_IsPlaying = false;
                    };
                };
            };
        }
    }
}