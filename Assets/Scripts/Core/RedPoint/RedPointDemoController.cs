using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class RedPointDemoController : UIBaseController
    {
        RedPointDemoView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/RedPointDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as RedPointDemoView;

            view.closeButton.onClick.AddListener(Close);
            view.redPointButton1.onClick.AddListener(() => { ClearRedPoint(ERedPointType.RedPointDemoSub1); });
            view.redPointButton2.onClick.AddListener(() => { ClearRedPoint(ERedPointType.RedPointDemoSub2); });
            view.redPointButton3.onClick.AddListener(() => { ClearRedPoint(ERedPointType.RedPointDemoSub3); });
        }

        private void ClearRedPoint(ERedPointType type)
        {
            RedPointManager.Instance.SetRedPointNum(type, 0);
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();
        }
    }
}