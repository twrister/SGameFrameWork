using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;
using UnityEngine.UI;

namespace SthGame
{
    public class AStarPathFindingController : PathFindingBaseController
    {
        AStarPathFindingView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/AStarPathFindingView";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as AStarPathFindingView;
        }

        public override void ShutDown()
        {
            base.ShutDown();
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            curMapData = new PathFindingMapData(20, 12);

            InitGrids();
        }

        protected override void HideCallBack()
        {
            base.HideCallBack();
        }

        protected override void UpdateGrids()
        {
            base.UpdateGrids();
        }

        protected override void OnClickGrid(int gridIndex)
        {
            if (gridIndex < gridCount)
            {
                // 0 1 2 之中切换，0是代价为1的格子，1是墙，2是代价为5的格子
                int state = curMapData[gridIndex];
                int typeCount = 2;
                curMapData[gridIndex] = (state + 1) % typeCount;
                DoSearch();
            }
        }

        protected override void DoSearch()
        {
            UpdateGrids();
        }
    }
}