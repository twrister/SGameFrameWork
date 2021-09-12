using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;
using UnityEngine.UI;

namespace SthGame
{
    public class BFSPathFindingController : PathFindingBaseController
    {
        BFSPathFindingView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/BFSPathFindingView";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as BFSPathFindingView;

            view.arrowToggle.onValueChanged.AddListener(OnArrowToggleChanged);
            
            view.earlyExitToggle.onValueChanged.AddListener(OnEarlyExitToggleChanged);

            view.movementCostToggle.onValueChanged.AddListener(OnMovementCostToggleChanged);

            view.arrowPrefab.CreatePool();
            view.arrowPrefab.SetActive(false);
        }

        public override void ShutDown()
        {
            base.ShutDown();

            view.arrowPrefab.DestroyPooled();
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

            UpdateArrows();
            UpdatePaths();
        }

        #region arrow display
        bool _showArrow = false;
        bool ShowArrow
        {
            get { return _showArrow; }
            set
            {
                _showArrow = value;
                UpdateArrows();
            }
        }

        void OnArrowToggleChanged(bool isOn)
        {
            ShowArrow = isOn;
        }

        List<GameObject> arrowList = new List<GameObject>();

        void UpdateArrows()
        {
            if (view == null) return;
            view.arrowParent.SetActive(ShowArrow);
            if (ShowArrow)
            {
                int arrowCount = 0;
                for (int i = 0; i < gridList.Count; i++)
                {
                    if (cameFromDict.ContainsKey(i))
                    {
                        arrowCount++;
                        while (arrowList.Count < arrowCount)
                        {
                            GameObject arrowGO = ObjectPool.Spawn(view.arrowPrefab, view.arrowParent.transform);
                            arrowGO.SetActive(true);
                            arrowGO.transform.localScale = Vector3.one;
                            arrowList.Add(arrowGO);
                        }
                        arrowList[arrowCount - 1].transform.position = gridList[i].transform.position;
                        float angle = GetArrowAngle(i, cameFromDict[i]);
                        arrowList[arrowCount - 1].transform.localRotation = Quaternion.Euler(0, 0, angle);
                    }
                }

                while (arrowList.Count > arrowCount)
                {
                    arrowList[arrowList.Count - 1].Recycle();
                    arrowList.RemoveAt(arrowList.Count - 1);
                }
            }
        }

        protected override void UpdateGridTexts()
        {
            int gridTextCount = 0;
            for (int i = 0; i < gridList.Count; i++)
            {
                if (costSoFarDict.ContainsKey(i))
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
                    gridTextList[gridTextCount - 1].text = costSoFarDict[i].ToString();
                }
            }

            while (gridTextCount < gridTextList.Count)
            {
                gridTextList[gridTextList.Count - 1].Recycle();
                gridTextList.RemoveAt(gridTextList.Count - 1);
            }
        }

        #endregion

        #region UI Logic

        bool _earlyExit = true;
        bool EarlyExit
        {
            get { return _earlyExit; }
            set
            {
                _earlyExit = value;
                DoSearch();
            }
        }

        void OnEarlyExitToggleChanged(bool isOn)
        {
            EarlyExit = isOn;
        }

        bool _hasMovementCost = false;
        bool HasMovementCost
        {
            get { return _hasMovementCost; }
            set
            {
                _hasMovementCost = value;
                DoSearch();
            }
        }

        void OnMovementCostToggleChanged(bool isOn)
        {
            HasMovementCost = isOn;
        }

        protected override void OnClickGrid(int gridIndex)
        {
            if (gridIndex < gridCount)
            {
                // 0 1 2 之中切换，0是代价为1的格子，1是墙，2是代价为5的格子
                int state = curMapData[gridIndex];
                int typeCount = HasMovementCost ? 3 : 2;
                curMapData[gridIndex] = (state + 1) % typeCount;
                DoSearch();
            }
        } 
        #endregion

        protected override void DoSearch()
        {
            DoBFS();

            UpdateGrids();
        }

        #region BFS
        SimplePriorityQueue<int> frontierPriorityQueue = new SimplePriorityQueue<int>();    // 储存探索边界的优先队列
        int[] neighborArray = new int[4];                                                   // 储存临时的探索边界
        Dictionary<int, int> cameFromDict = new Dictionary<int, int>();                     // 储存每个格子的来向
        Dictionary<int, int> costSoFarDict = new Dictionary<int, int>();                    // 储存所有格子的到起点的最少花费

        void DoBFS()
        {
            if (mapWidth == 0 || mapHeight == 0) return;
            if (view.player.Index >= gridCount) return;

            // 储存边界的优先队列
            frontierPriorityQueue.Clear();
            frontierPriorityQueue.Enqueue(view.player.Index, 0);

            cameFromDict.Clear();

            costSoFarDict.Clear();
            costSoFarDict[view.player.Index] = 0;

            int curStep = 0;
            int next = 0;
            int newCost = 0;
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
                        if (next == -1) continue;               // 四周
                        if (curMapData.IsBlock(next)) continue; // 墙

                        int toNextCost = HasMovementCost ? (curMapData[next] == PathFindingGridView.NORMAL ? 1 : 5) : 1;
                        newCost = costSoFarDict[curIndex] + toNextCost;
                        if (!costSoFarDict.ContainsKey(next) || newCost < costSoFarDict[next])  // 没被探索
                        {
                            costSoFarDict[next] = newCost;
                            frontierPriorityQueue.Enqueue(next, newCost);       // 步数少的优先探索
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
            if (HasMovementCost)
            {
                for (int i = 0; i < curMapData.showArray.Length; i++)
                {
                    curMapData.showArray[i] = curMapData[i];
                }
            }
            else
            {
                // 标记已探索
                for (int i = 0; i < curMapData.showArray.Length; i++)
                {
                    curMapData.showArray[i] = cameFromDict.ContainsKey(i) ? PathFindingGridView.REACHED : curMapData.showArray[i];
                }
                // 标记边界
                while (frontierPriorityQueue.Count > 0)
                {
                    int index = frontierPriorityQueue.Dequeue();
                    curMapData.showArray[index] = PathFindingGridView.FRONTIER;
                }
            }
        }
        #endregion
    }
}