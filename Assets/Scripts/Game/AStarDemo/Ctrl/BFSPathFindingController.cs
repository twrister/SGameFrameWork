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

            view.movementCostToggle.onValueChanged.AddListener(OnMovementCostToggleChanged);

            view.arrowPrefab.CreatePool();
            view.pathPrefab.CreatePool();
        }

        public override void ShutDown()
        {
            base.ShutDown();

            view.arrowPrefab.DestroyPooled();
            view.pathPrefab.DestroyPooled();
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

            UpdateMapNum();
            UpdateArrows();
            UpdatePaths();
        }

        void UpdateMapNum()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].indexText.text = costSoFarDict.ContainsKey(i) ? costSoFarDict[i].ToString() : "";
            }
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
                            arrowList.Add(GetOneArrow());
                        }
                        arrowList[arrowCount - 1].transform.position = gridList[i].transform.position;
                        float angle = GetArrowAngle(i, cameFromDict[i]);
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

        List<GameObject> pathGOList = new List<GameObject>();
        void UpdatePaths()
        {
            if (view == null) return;

            while (pathGOList.Count > pathList.Count)
            {
                pathGOList[pathGOList.Count - 1].Recycle();
                pathGOList.RemoveAt(pathGOList.Count - 1);
            }

            while (pathGOList.Count < pathList.Count)
            {
                GameObject path = view.pathPrefab.Spawn(view.pathParent.transform);
                path.SetActive(true);
                path.transform.localScale = Vector3.one;
                pathGOList.Add(path);
            }

            if (pathList.Count > 0)
            {
                pathList.Add(view.player.Index);
                for (int i = 0; i < pathList.Count - 1; i++)
                {
                    int curGrid = pathList[i];
                    int cameFrom = pathList[i + 1];
                    //int gridIdx = pathList[i];
                    pathGOList[i].transform.position = gridList[curGrid].transform.position;
                    float angle = GetArrowAngle(curGrid, cameFrom);
                    pathGOList[i].name = curGrid.ToString();
                    pathGOList[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
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

        bool _hasMovementCost = true;
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
        SimplePriorityQueue<int> frontierPriorityQueue = new SimplePriorityQueue<int>();    // 储存探索边界的优先队列
        //Queue<int> frontierQueue = new Queue<int>();    // 储存探索边界
        int[] neighborArray = new int[4];               // 储存临时的探索边界
        //int[] cameFromArray = new int[0];               // 储存所有格子的came from
        int[] costSoFar = new int[0];                   // 储存所有格子的到起点的最少花费
        List<int> pathList = new List<int>();           // 储存路径

        Dictionary<int, int> cameFromDict = new Dictionary<int, int>();
        Dictionary<int, int> costSoFarDict = new Dictionary<int, int>();

        void DoBFS()
        {
            if (mapWidth == 0 || mapHeight == 0) return;
            if (view.player.Index >= gridCount) return;

            // 储存边界的优先队列
            frontierPriorityQueue.Clear();
            frontierPriorityQueue.Enqueue(view.player.Index, 0);

            // 储存边界的队列
            //frontierQueue.Clear();
            //frontierQueue.Enqueue(view.player.Index);

            // cameFromArray记录每个grid的来向，-1表示当前为block
            //cameFromArray = new int[mapWidth * mapHeight];
            //cameFromArray[view.player.Index] = -1;
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
                        if (next == -1) continue;   // 四周
                        if (curMapData.IsBlock(next)) continue; // 墙

                        newCost = costSoFarDict[curIndex] + 1;
                        if (!costSoFarDict.ContainsKey(next) || newCost < costSoFarDict[next])  // 没被探索
                        {
                            costSoFarDict[next] = newCost;
                            frontierPriorityQueue.Enqueue(next, newCost);       // 步数少的优先探索
                            //cameFromArray[next] = curIndex + 1;
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

            // 标记已探索
            curMapData.ClearShowArray();
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