using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class AStarDemoController : UIBaseController
    {
        AStarDemoView view;
        List<AStarGridView> grigList = new List<AStarGridView>();

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

            InitGrids(10, 10);
        }

        private void InitGrids(int width, int height)
        {
            grigList.Clear();
            //view

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    AStarGridView grid = GameObject.Instantiate<AStarGridView>(view.gridPrefab, view.gridPrefab.transform);
                    //grid.posX = i;
                    //grid.posY = j;
                }
            }
        }
    }
}