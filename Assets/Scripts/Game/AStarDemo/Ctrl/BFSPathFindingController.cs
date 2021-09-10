using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;

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

            view.arrowPrefab.CreatePool();
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
                for (int i = 0; i < cameFromArray.Length; i++)
                {
                    if (cameFromArray[i] > 0)
                    {
                        arrowCount++;
                        while (arrowList.Count < arrowCount)
                        {
                            arrowList.Add(GetOneArrow());
                        }
                        arrowList[arrowCount - 1].transform.position = gridList[i].transform.position;
                        float angle = GetArrowAngle(i, cameFromArray[i] - 1);   // came from 赋值时+1了，读取时-1
                        arrowList[arrowCount - 1].transform.localRotation = Quaternion.Euler(0, 0, angle);
                    }
                }

                while (arrowList.Count > arrowCount)
                {
                    RecycleArrow(arrowList[arrowList.Count - 1]);
                    arrowList.RemoveAt(arrowList.Count - 1);
                }
            }
        }

        float GetArrowAngle(int curGrid, int cameFromGrid)
        {
            int delta = cameFromGrid - curGrid;
            if (delta == mapHeight)
                return 0f;
            else if (delta == -mapHeight)
                return 180f;
            else if (delta == 1)
                return 90f;
            else
                return 270f;
        }

        // arrow pool
        Stack<GameObject> arrowStack = new Stack<GameObject>();

        GameObject GetOneArrow()
        {
            GameObject arrowGO = ObjectPool.Spawn(view.arrowPrefab, view.arrowParent.transform);
            arrowGO.SetActive(true);
            arrowGO.transform.localScale = Vector3.one;
            return arrowGO;
        }

        void RecycleArrow(GameObject arrow)
        {
            ObjectPool.Recycle(arrow);
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

        protected override void PathFindingOnClickGrid(object[] ps)
        {
            int index = (int)ps[0];
            if (index < gridCount)
            {
                int state = curMapData[index];
                curMapData[index] = 1 - state;  // 目前只有墙跟普通格子
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
        Queue<int> frontierQueue = new Queue<int>();    // 储存探索边界
        int[] neighborArray = new int[4];               // 储存临时的探索边界
        int[] cameFromArray = new int[0];               // 储存所有格子的came from
        int[] costSoFar = new int[0];                   // 储存所有格子的到起点的最少花费
        List<int> pathList = new List<int>();           // 储存路径

        void DoBFS()
        {
            if (mapWidth == 0 || mapHeight == 0) return;
            if (view.player.Index >= gridCount) return;

            // 储存边界的队列
            frontierQueue.Clear();
            frontierQueue.Enqueue(view.player.Index);

            // cameFromArray记录每个grid的来向，-1表示当前为block
            cameFromArray = new int[mapWidth * mapHeight];
            cameFromArray[view.player.Index] = -1;

            // ShowStep 显示步数的开关
            int curStep = 0;
            while (!ShowStep || curStep < Step)
            {
                if (frontierQueue.Count > 0)
                {
                    int curIndex = frontierQueue.Dequeue();

                    // 边界到达目标点，提前结束
                    if (EarlyExit && curIndex == view.target.Index) break;

                    CalcNeighborIndexs(ref neighborArray, curIndex);
                    for (int i = 0; i < neighborArray.Length; i++)
                    {
                        // 找到邻边有效的,并且没被标记的grid
                        if (neighborArray[i] >= 0 && cameFromArray[neighborArray[i]] == 0)
                        {
                            if (curMapData.IsBlock(neighborArray[i]))
                            {
                                cameFromArray[neighborArray[i]] = -1;               // 临边是block，came from也标为-1
                            }
                            else
                            {
                                frontierQueue.Enqueue(neighborArray[i]);
                                cameFromArray[neighborArray[i]] = curIndex + 1;         // 标记来自哪个grid，记录Index +1，避免默认指向第一个格子的情况
                            }
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
            if (cameFromArray[current] > 0)   // 说明已探索到target
            {
                while (current != view.player.Index)
                {
                    pathList.Add(current);
                    current = cameFromArray[current] - 1;   // 前面记录的时候 +1了，这里读取时 -1
                }
            }

            // 标记已探索
            curMapData.ClearShowArray();
            for (int i = 0; i < cameFromArray.Length; i++)
            {
                curMapData.showArray[i] = cameFromArray[i] != 0 ? PathFindingGridView.REACHED : curMapData.showArray[i];
            }
            // 标记边界
            while (frontierQueue.Count > 0)
            {
                int index = frontierQueue.Dequeue();
                curMapData.showArray[index] = PathFindingGridView.FRONTIER;
            }
            // 标记路线
            for (int i = 0; i < pathList.Count; i++)
            {
                curMapData.showArray[pathList[i]] = PathFindingGridView.PATH;
            }
        }

        void CalcNeighborIndexs(ref int[] neighbors, int curIndex)
        {
            // 错开求邻，single为true则顺时求邻，false则逆时
            int x = curIndex / mapHeight;
            int y = curIndex % mapHeight;
            bool single = (x + y) % 2 == 1;
            neighbors[single ? 0 : 3] = GetRightwardGridIndex(curIndex);
            neighbors[single ? 1 : 2] = GetDownwardGridIndex(curIndex);
            neighbors[single ? 2 : 1] = GetLefttwardGridIndex(curIndex);
            neighbors[single ? 3 : 0] = GetUpwardGridIndex(curIndex);
        }
        #endregion
    }
}