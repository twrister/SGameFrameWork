using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class AStarDemoController : UIBaseController
    {
        AStarDemoView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/AStarDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as AStarDemoView;
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            InitGrids();
        }

        private void InitGrids()
        {
            //view
        }
    }
}