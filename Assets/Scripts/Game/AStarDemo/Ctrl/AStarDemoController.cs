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
                //DoBFS();
            }
        }

        AStarMapData curMapData;
        int gridEdge { get { return curMapData.edgeLen; } }
        int mapWidth { get { return curMapData.mapWidth; } }
        int mapHeight { get { return curMapData.mapHeight; } }
        int gridCount { get { return curMapData.mapWidth * curMapData.mapHeight; } }

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
        }

        void UpdateGrids()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                int tag = gridTagArray[i];
                if (tag == 0)
                {
                    gridList[i].gridImage.color = Color.white;
                }
                else if (tag == 1)
                {
                    gridList[i].gridImage.color = Color.gray;
                }
                else if (tag == 2)
                {
                    gridList[i].gridImage.color = Color.blue;
                }
            }
        }

        void OnPlayerPosChanged()
        {
            //DoBFS();
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
            if (index < gridList.Count)
            {
                int preGridType = (int)gridList[index].GridType;
                gridList[index].GridType = (EWayFindingGridType)((preGridType + 1) % (int)EWayFindingGridType.__Count);
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

        bool IsBlock(int index)
        {
            var grid = GetGridByIndex(index);
            if (grid == null) return true;
            return grid.GridType == EWayFindingGridType.Block;
        }

        AStarGridView GetGridByIndex(int index)
        {
            if (gridList.Count == 0 || index < 0 || index >= gridList.Count) return null;
            return gridList[index];
        }

        AStarGridView GetUpwardGrid(AStarGridView grid)
        {
            if (grid == null || grid.posY == mapHeight - 1) return null;
            int index = grid.Index + 1;
            return GetGridByIndex(index);
        }

        AStarGridView GetDownwardGrid(AStarGridView grid)
        {
            if (grid == null || grid.posY == 0) return null;
            int index = grid.Index - 1;
            return GetGridByIndex(index);
        }

        AStarGridView GetLeftwardGrid(AStarGridView grid)
        {
            if (grid == null || grid.posX == 0) return null;
            int index = grid.Index - grid.MapHeight;
            return GetGridByIndex(index);
        }

        AStarGridView GetRightwardGrid(AStarGridView grid)
        {
            if (grid == null || grid.posY == mapHeight - 1) return null;
            int index = grid.Index + grid.MapHeight;
            return GetGridByIndex(index);
        }

        int GetRightwardGridIndex(int index)
        {
            if (index < 0 || index >= mapWidth * mapHeight) return -1;
            return index + mapHeight;
        }

        void SetGridColor(int index)
        {
            var grid = GetGridByIndex(index);
            grid.gridImage.color = Color.blue;
        }

        #endregion

        #region BFS
        //List<int> frontierList = new List<int>();
        int[] gridTagArray;
        Queue<int> frontierQueue;
        //List<int> reachedList = new List<int>();
        //WaitForSeconds wait1s = new WaitForSeconds(1);
        bool[] reachedArray;

        void DoBFS()
        {
            if (mapWidth == 0 || mapHeight == 0) return;

            frontierQueue = new Queue<int>();

            frontierQueue.Enqueue(view.player.Index);
            List<int> neighbors = new List<int>();

            gridTagArray = new int[mapWidth * mapHeight];
            reachedArray = new bool[mapWidth * mapHeight];

            reachedArray[view.player.Index] = true;

            for (int i = 0; i < Step; i++)
            {
                if (frontierQueue.Count > 0)
                {
                    int curIndex = frontierQueue.Dequeue();
                    neighbors = GetNeighborIndexs(curIndex);

                    foreach (var nei in neighbors)
                    {
                        if (!reachedArray[nei])
                        {
                            frontierQueue.Enqueue(nei);
                            reachedArray[nei] = true;
                        }
                    }
                }
            }

            for (int i = 0; i < gridTagArray.Length; i++)
            {
                if (reachedArray[i])
                {
                    gridTagArray[i] = 1;
                }
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                gridTagArray[neighbors[i]] = 2;
            }

            UpdateGrids();
        }

        List<int> GetNeighborIndexs(int curIndex)
        {
            var result = new List<int>();
            var cur = GetGridByIndex(curIndex);
            var right = GetRightwardGrid(cur);
            if (right != null) result.Add(right.Index);
            var down = GetDownwardGrid(cur);
            if (down != null) result.Add(down.Index);
            var left = GetLeftwardGrid(cur);
            if (left != null) result.Add(left.Index);
            var up = GetUpwardGrid(cur);
            if (up != null) result.Add(up.Index);
            return result;
        }

        #endregion
    }
}