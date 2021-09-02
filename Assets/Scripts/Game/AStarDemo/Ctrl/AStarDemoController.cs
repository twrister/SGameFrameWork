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
            view.stepSlider.maxValue = 100f;
            view.stepSlider.onValueChanged.AddListener(OnStepSliderValueChanged);
            view.stepSlider.value = 5f;

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

            view.player.InitMovableItem(new Vector2Int(0, 0), gridEdge, mapWidth, mapHeight, view.gridParent.transform, OnPlayerPosChanged);
            view.target.InitMovableItem(new Vector2Int(mapWidth - 1, mapHeight - 1), gridEdge, mapWidth, mapHeight, view.gridParent.transform);

            UpdateGrids();
        }

        void UpdateGrids()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].SetGridState(curMapData.showArray[i], curMapData.IsBlock(i));
            }
        }

        void OnPlayerPosChanged()
        {
            DoBFS();
        }

        void OnTargetPosChanged()
        {
            //DoBFS();
        }

        void OnStepSliderValueChanged(float value)
        {
            //Logger.Log("change : {0}", value);
            Step = (int)value;
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

        //bool IsBlock(int index)
        //{
        //    var grid = GetGridByIndex(index);
        //    if (grid == null) return true;
        //    return grid.GridType == EWayFindingGridType.Block;
        //}

        //AStarGridView GetGridByIndex(int index)
        //{
        //    if (gridList.Count == 0 || index < 0 || index >= gridList.Count) return null;
        //    return gridList[index];
        //}

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

        //void SetGridColor(int index)
        //{
        //    var grid = GetGridByIndex(index);
        //    grid.gridImage.color = Color.blue;
        //}

        #endregion

        #region BFS
        Queue<int> frontierQueue = new Queue<int>();
        int[] neighborArray = new int[4];
        bool[] reachedArray;

        void DoBFS()
        {
            if (mapWidth == 0 || mapHeight == 0) return;
            if (view.player.Index >= gridCount) return; 

            frontierQueue.Clear();
            frontierQueue.Enqueue(view.player.Index);

            reachedArray = new bool[mapWidth * mapHeight];
            reachedArray[view.player.Index] = true;

            int curStep = 0;
            while (curStep < Step)
            {
                if (frontierQueue.Count > 0)
                {
                    int curIndex = frontierQueue.Dequeue();
                    //neighbors = GetNeighborIndexs(curIndex);
                    CalcNeighborIndexs(ref neighborArray, curIndex);
                    for (int i = 0; i < neighborArray.Length; i++)
                    {
                        if (neighborArray[i] >= 0 && !reachedArray[neighborArray[i]])
                        {
                            if (curMapData.IsBlock(neighborArray[i]))
                            {
                                reachedArray[neighborArray[i]] = true;
                            }
                            else
                            {
                                frontierQueue.Enqueue(neighborArray[i]);
                                reachedArray[neighborArray[i]] = true;
                            }
                        }
                    }
                }
                curStep++;
            }

            curMapData.ClearShowArray();
            for (int i = 0; i < reachedArray.Length; i++)
            {
                curMapData.showArray[i] = reachedArray[i] ? AStarGridView.REACHED : curMapData.showArray[i];
            }

            while (frontierQueue.Count > 0)
            {
                int index = frontierQueue.Dequeue();
                curMapData.showArray[index] = AStarGridView.FRONTIER;
            }

            UpdateGrids();
        }

        void CalcNeighborIndexs(ref int[] neighbors, int curIndex)
        {
            neighbors[0] = GetRightwardGridIndex(curIndex);
            neighbors[1] = GetDownwardGridIndex(curIndex);
            neighbors[2] = GetLefttwardGridIndex(curIndex);
            neighbors[3] = GetUpwardGridIndex(curIndex);
        }

        #endregion
    }
}