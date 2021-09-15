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

            view.greedyToggle.onValueChanged.AddListener(OnGreedyToggleChanged);
        }

        public override void ShutDown()
        {
            base.ShutDown();
        }

        bool _greedy = false;
        bool Greedy
        {
            get { return _greedy; }
            set
            {
                _greedy = value;
                DoSearch();
            }
        }

        void OnGreedyToggleChanged(bool isOn)
        {
            Greedy = isOn;
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
            DoGreedyBestSearch();

            UpdateGrids();
        }

        SimplePriorityQueue<int> frontierPriorityQueue = new SimplePriorityQueue<int>();    // 储存探索边界的优先队列
        int[] neighborArray = new int[4];                                                   // 储存临时的探索边界
        Dictionary<int, int> cameFromDict = new Dictionary<int, int>();                     // 储存每个格子的来向
        Dictionary<int, int> costSoFarDict = new Dictionary<int, int>();                    // 储存所有格子的到起点的最少花费

        Dictionary<int, int> heuristicDict = new Dictionary<int, int>();
        Dictionary<int, int> crossDict = new Dictionary<int, int>();
        void DoGreedyBestSearch()
        {
            if (mapWidth == 0 || mapHeight == 0) return;
            if (view.player.Index >= gridCount) return;

            // 储存边界的优先队列
            frontierPriorityQueue.Clear();
            frontierPriorityQueue.Enqueue(view.player.Index, 0);

            cameFromDict.Clear();

            costSoFarDict.Clear();
            costSoFarDict[view.player.Index] = 0;

            heuristicDict.Clear();

            int curStep = 0;
            int next = 0;
            float priority = 0;
            int newCost = 0;
            int heuristic = 0;
            int cross = 0;
            while (!ShowStep || curStep < Step)
            {
                if (frontierPriorityQueue.Count > 0)
                {
                    int curIndex = frontierPriorityQueue.Dequeue();

                    // 边界到达目标点，提前结束
                    if (EarlyExit && curIndex == view.target.Index) break;

                    CalcNeighborIndexs(ref neighborArray, curIndex);
                    for (int i = 0; i < neighborArray.Length; i++)
                    {
                        next = neighborArray[i];
                        if (next == -1) continue;                   // 四周
                        if (curMapData.IsBlock(next)) continue;     // 墙

                        int toNextCost = 1;
                        newCost = costSoFarDict[curIndex] + toNextCost;
                        if (!costSoFarDict.ContainsKey(next) || newCost < costSoFarDict[next])  // 没被探索
                        {
                            costSoFarDict[next] = newCost;
                            heuristic = Heuristic(view.target.Index, next);
                            heuristicDict[next] = heuristic;
                            cross = Cross(next, view.target.Index, view.player.Index);
                            crossDict[next] = cross;
                            if (Greedy)
                            {
                                priority = heuristic;
                            }
                            else
                            {
                                // cross值越小，表示线路方向越接近起点到终点，更优先地搜索
                                priority = (newCost + heuristic) * 1000 + cross;
                            }
                            frontierPriorityQueue.Enqueue(next, priority);       // 步数少的优先探索
                            cameFromDict[next] = curIndex;
                        }
                    }
                }
                else
                {
                    break;
                }
                if (ShowStep) curStep++;
            }

            // 通过cameFromArray，找出路径
            pathList.Clear();
            int current = view.target.Index;
            if (cameFromDict.ContainsKey(current))   // 说明已探索到target
            {
                while (current != view.player.Index)
                {
                    pathList.Add(current);
                    current = cameFromDict[current];
                }
            }

            curMapData.ClearShowArray();

            // 标记已探索
            for (int i = 0; i < curMapData.showArray.Length; i++)
            {
                curMapData.showArray[i] = cameFromDict.ContainsKey(i) ? PathFindingGridView.REACHED : curMapData.showArray[i];
            }
        }

        protected override void UpdateGridTexts()
        {
            //ShowGridTexts(priorityDict);
            int gridTextCount = 0;
            for (int i = 0; i < gridList.Count; i++)
            {
                if (heuristicDict.ContainsKey(i) && costSoFarDict.ContainsKey(i) && crossDict.ContainsKey(i))
                {
                    gridTextCount++;
                    if (gridTextList.Count < gridTextCount)
                    {
                        Text text = view.gridTextPrefab.Spawn(view.gridTextParent);
                        text.gameObject.SetActive(true);
                        text.transform.localScale = Vector3.one;
                        gridTextList.Add(text);
                    }
                    gridTextList[gridTextCount - 1].transform.position = gridList[i].transform.position;
                    gridTextList[gridTextCount - 1].text = Greedy ? 
                        string.Format("<color=#008888>{0}</color>", heuristicDict[i]) : 
                        string.Format("{0} {1} {2}\n<color=#008888>{3}</color>",
                        costSoFarDict[i].ToString(),
                    heuristicDict[i].ToString(),
                    crossDict[i].ToString(),
                    ((costSoFarDict[i] + heuristicDict[i]) * 1000 + crossDict[i]).ToString());
                }
            }

            while (gridTextCount < gridTextList.Count)
            {
                gridTextList[gridTextList.Count - 1].Recycle();
                gridTextList.RemoveAt(gridTextList.Count - 1);
            }
        }
    }
}