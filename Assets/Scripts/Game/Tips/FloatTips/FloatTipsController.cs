﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SthGame
{
    public class FloatTipsController : UIBaseController
    {
        FloatTipsView view;
        Queue<Tuple<string, float>> contentQueue = new Queue<Tuple<string, float>>();
        
        bool isPlaying = false;

        private List<SingleFloatTips> m_TipsList = new List<SingleFloatTips>();

        private bool m_FloatImmediately = true;

        protected override string GetResourcePath()
        {
            return "Prefabs/FloatTipsView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as FloatTipsView;
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();
        }

        public void ShowFloatTips(string content, float duration = 1.5f)
        {
            if (string.IsNullOrEmpty(content)) return;

            contentQueue.Enqueue(new Tuple<string, float>(content, duration));
            TryPopOneTips();
        }

        private void TryPopOneTips()
        {
            if (m_FloatImmediately && isPlaying) return;

            PopOneTips();
        }

        private void PopOneTips()
        {
            if (contentQueue.Count == 0)
            {
                isPlaying = false;
                return;
            }

            isPlaying = true;
            var tuple = contentQueue.Dequeue();
            string content = tuple.Item1;
            float duration = tuple.Item2;
            var Tips = GetOneFloatTips();

            for (int i = 0; i < m_TipsList.Count; i++)
            {
                m_TipsList[i].MoveToEndImmediately();
            }
            
            Tips.Show(content, duration, PopOneTips, RecycleFloatTips);
            m_TipsList.Add(Tips);
        }

        #region simple pool

        Stack<SingleFloatTips> tipsStack = new Stack<SingleFloatTips>();

        SingleFloatTips GetOneFloatTips()
        {
            SingleFloatTips tips = null;

            if (tipsStack.Count == 0)
            {
                view.tipsPrefab.gameObject.SetActive(true);
                tips = GameObject.Instantiate<SingleFloatTips>(view.tipsPrefab, view.centerTrans);
                view.tipsPrefab.gameObject.SetActive(false);
                tips.Reset();
            }
            else
            {
                tips = tipsStack.Pop();
            }

            if (tips != null)
            {
                tips.gameObject.SetActive(true);
            }

            return tips;
        }

        void RecycleFloatTips(SingleFloatTips tips)
        {
            if (tips != null)
            {
                m_TipsList.Remove(tips);
                tips.Reset();
                tipsStack.Push(tips);
            }
        }

        #endregion
    }
}