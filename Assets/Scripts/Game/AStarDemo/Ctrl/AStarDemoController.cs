using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class AStarDemoController : UIBaseController
    {
        AStarDemoView view;
        List<AStarGridView> gridList = new List<AStarGridView>();

        protected override string GetResourcePath()
        {
            return "Prefabs/AStarDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as AStarDemoView;

            view.closeButton.onClick.AddListener(Close);
            view.stepSlider.maxValue = 50f;
            view.stepSlider.onValueChanged.AddListener(OnStepSliderValueChanged);
            view.stepSlider.value = 5f;

            view.arrowToggle.onValueChanged.AddListener(OnArrowToggleChanged);
            view.stepToggle.onValueChanged.AddListener(OnStepToggleChanged);
            view.earlyExitToggle.onValueChanged.AddListener(OnEarlyExitToggleChanged);

            GlobalEventSystem.Instance.Bind(EventId.aStarOnClickGrid, AStarOnClickGrid);
        }

        public override void ShutDown()
        {
            base.ShutDown();

            GlobalEventSystem.Instance.UnBindAll(EventId.aStarOnClickGrid);
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            curMapData = new AStarMapData(20, 12);
            InitGrids();
        }

        protected override void HideCallBack()
        {
            base.HideCallBack();

        }

        AStarMapData curMapData;
        int gridEdge { get { return curMapData == null ? 0 : curMapData.EdgeLen; } }
        int mapWidth { get { return curMapData == null ? 0 : curMapData.MapWidth; } }
        int mapHeight { get { return curMapData == null ? 0 : curMapData.MapHeight; } }
        int gridCount { get { return curMapData == null ? 0 : curMapData.GridCount; } }

        private void InitGrids()
        {
            if (mapWidth < 1 || mapHeight < 1) return;

            // 回收Grid
            while (gridList.Count > gridCount)
            {
                RecycleGrid(gridList[gridList.Count - 1]);
                gridList.RemoveAt(gridList.Count - 1);
            }

            // 生成Grid
            while (gridList.Count < gridCount)
            {
                gridList.Add(GetOneGrid());
            }

            // 初始化Grid位置
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    int index = x * mapHeight + y;
                    gridList[index].InitPos(x, y, gridEdge, mapWidth, mapHeight);
                }
            }

            // 初始起点、终点位置
            Vector2Int playerPos = new Vector2Int(Mathf.Min(4, mapWidth - 1), Mathf.Min(4, mapHeight - 1));
            view.player.InitMovableItem(playerPos, gridEdge, mapWidth, mapHeight, view.gridParent.transform, OnPlayerPosChanged);
            Vector2Int targetPos = new Vector2Int(Mathf.Max(0, mapWidth - 4) , Mathf.Max(0, mapHeight - 4));
            view.target.InitMovableItem(targetPos, gridEdge, mapWidth, mapHeight, view.gridParent.transform, OnTargetPosChanged);

            view.stepSlider.maxValue = (float)gridCount;

            DoBFS();
        }

        #region display
        void UpdateGrids()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].SetGridState(curMapData.showArray[i], curMapData.IsBlock(i));
            }

            UpdateArrows();
        }

        List<GameObject> arrowList = new List<GameObject>();
        void UpdateArrows()
        {
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

        #endregion

        #region UI Logic
        void OnPlayerPosChanged()
        {
            DoBFS();
        }

        void OnTargetPosChanged()
        {
            DoBFS();
        }

        int _step;
        int Step
        {
            get { return _step; }
            set
            {
                _step = value;
                DoBFS();
            }
        }

        bool _showArrow = true;
        bool ShowArrow
        {
            get { return _showArrow; }
            set
            {
                _showArrow = value;
                UpdateArrows();
            }
        }

        bool _showStep = true;
        bool ShowStep
        {
            get { return _showStep; }
            set
            {
                _showStep = value;
                DoBFS();
            }
        }

        bool _earlyExit = true;
        bool EarlyExit
        {
            get { return _earlyExit; }
            set
            {
                _earlyExit = value;
                DoBFS();
            }
        }

        void OnStepSliderValueChanged(float value)
        {
            Step = (int)value;
        }

        void OnArrowToggleChanged(bool isOn)
        {
            ShowArrow = isOn;
        }

        void OnStepToggleChanged(bool isOn)
        {
            ShowStep = isOn;
        }

        void OnEarlyExitToggleChanged(bool isOn)
        {
            EarlyExit = isOn;
        }

        private void AStarOnClickGrid(object[] ps)
        {
            int index = (int)ps[0];
            if (index < gridCount)
            {
                int state = curMapData[index];
                curMapData[index] = 1 - state;  // 目前只有墙跟普通格子
                DoBFS();
            }
        } 
        #endregion

        #region Grid pool

        Stack<AStarGridView> gridStack = new Stack<AStarGridView>();

        AStarGridView GetOneGrid()
        {
            AStarGridView grid = null;

            if (gridStack.Count == 0)
            {
                view.gridPrefab.gameObject.SetActive(true);
                grid = GameObject.Instantiate<AStarGridView>(view.gridPrefab, view.gridParent.transform);
                view.gridPrefab.gameObject.SetActive(false);
            }
            else
            {
                grid = gridStack.Pop();
            }

            if (grid != null)
            {
                grid.gameObject.SetActive(true);
            }

            return grid;
        }

        void RecycleGrid(AStarGridView grid)
        {
            if (grid != null)
            {
                grid.Reset();
                gridStack.Push(grid);
            }
        }

        #endregion

        #region Arrow pool

        Stack<GameObject> arrowStack = new Stack<GameObject>();

        GameObject GetOneArrow()
        {
            GameObject arrowGO = null;

            if (arrowStack.Count == 0)
            {
                view.arrowPrefab.gameObject.SetActive(true);
                arrowGO = GameObject.Instantiate<GameObject>(view.arrowPrefab, view.arrowParent.transform);
                view.arrowPrefab.gameObject.SetActive(false);
            }
            else
            {
                arrowGO = arrowStack.Pop();
            }

            if (arrowGO != null)
            {
                arrowGO.gameObject.SetActive(true);
            }

            return arrowGO;
        }

        void RecycleArrow(GameObject arrow)
        {
            if (arrow != null)
            {
                arrow.SetActive(false);
                arrowStack.Push(arrow);
            }
        }

        #endregion

        #region Common Logic
        public static void SetLocalPosByGridPos(Transform trans, int gridX, int gridY, int edge, int tX, int tY)
        {
            float localPosX = (gridX - (tX - 1) / 2f) * edge;
            float localPosY = (gridY - (tY - 1) / 2f) * edge;
            trans.localPosition = new Vector3(localPosX, localPosY);
        }

        public static Vector2Int GetGridPosByLocalPos(float localX, float localY, int edge, int tX, int tY)
        {
            Vector2Int gridPos = Vector2Int.zero;

            gridPos.x = Mathf.Min(tX - 1, Mathf.Max(0, Mathf.RoundToInt(localX / (float)edge + ((tX - 1) / 2f))));
            gridPos.y = Mathf.Min(tY - 1, Mathf.Max(0, Mathf.RoundToInt(localY / (float)edge + ((tY - 1) / 2f))));

            return gridPos;
        }

        int GetRightwardGridIndex(int index)
        {
            if (index < 0 || index >= (mapWidth - 1) * mapHeight) return -1;
            return index + mapHeight;
        }

        int GetDownwardGridIndex(int index)
        {
            if (index < 0 || index >= mapWidth * mapHeight || mapHeight == 0 || index % mapHeight == 0) return -1;
            return index - 1;
        }

        int GetLefttwardGridIndex(int index)
        {
            if (index < mapHeight || index >= mapWidth * mapHeight) return -1;
            return index - mapHeight;
        }

        int GetUpwardGridIndex(int index)
        {
            if (index < 0 || index >= mapWidth * mapHeight || mapHeight == 0 || index % mapHeight == (mapHeight - 1)) return -1;
            return index + 1;
        }

        #endregion

        #region BFS
        Queue<int> frontierQueue = new Queue<int>();
        int[] neighborArray = new int[4];       // 储存临时的探索边界
        int[] cameFromArray = new int[0];       // 储存所有格子的came from
        List<int> pathList = new List<int>();   // 储存路径

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
                curMapData.showArray[i] = cameFromArray[i] != 0 ? AStarGridView.REACHED : curMapData.showArray[i];
            }
            // 标记边界
            while (frontierQueue.Count > 0)
            {
                int index = frontierQueue.Dequeue();
                curMapData.showArray[index] = AStarGridView.FRONTIER;
            }
            // 标记路线
            for (int i = 0; i < pathList.Count; i++)
            {
                curMapData.showArray[pathList[i]] = AStarGridView.PATH;
            }

            UpdateGrids();
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