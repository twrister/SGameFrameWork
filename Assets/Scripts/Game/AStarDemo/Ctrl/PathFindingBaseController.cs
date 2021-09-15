using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SthGame
{
    public abstract class PathFindingBaseController : UIBaseController
    {
        PathFindingBaseView view;
        protected List<PathFindingGridView> gridList = new List<PathFindingGridView>();

        protected PathFindingMapData curMapData;

        protected int gridEdge { get { return curMapData == null ? 0 : curMapData.EdgeLen; } }
        protected int mapWidth { get { return curMapData == null ? 0 : curMapData.MapWidth; } }
        protected int mapHeight { get { return curMapData == null ? 0 : curMapData.MapHeight; } }
        protected int gridCount { get { return curMapData == null ? 0 : curMapData.GridCount; } }

        int _step;
        protected int Step
        {
            get { return _step; }
            set
            {
                _step = value;
                DoSearch();
            }
        }

        bool _showStep;
        protected bool ShowStep
        {
            get { return _showStep; }
            set
            {
                _showStep = value;
                DoSearch();
            }
        }

        bool _earlyExit = true;
        protected bool EarlyExit
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

        public override void Init()
        {
            base.Init();
            view = UINode as PathFindingBaseView;

            view.closeButton.onClick.AddListener(Close);
            view.stepSlider.maxValue = 50f;
            view.stepSlider.onValueChanged.AddListener(OnStepSliderValueChanged);
            view.stepSlider.value = 10f;

            view.stepToggle.onValueChanged.AddListener(OnStepToggleChanged);
            view.earlyExitToggle.onValueChanged.AddListener(OnEarlyExitToggleChanged);

            GlobalEventSystem.Instance.Bind(EventId.aStarOnClickGrid, PathFindingOnClickGrid);

            view.gridTextPrefab.CreatePool();
            view.gridTextPrefab.gameObject.SetActive(false);

            view.pathPrefab.CreatePool();
            view.pathPrefab.SetActive(false);
        }

        public override void ShutDown()
        {
            base.ShutDown();

            GlobalEventSystem.Instance.UnBindAll(EventId.aStarOnClickGrid);

            view.gridTextPrefab.DestroyPooled();
            view.pathPrefab.DestroyPooled();
        }

        protected virtual void InitGrids()
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
            view.player.InitMovableItem(playerPos, gridEdge, mapWidth, mapHeight, view.gridParent, OnPlayerPosChanged);
            Vector2Int targetPos = new Vector2Int(Mathf.Max(0, mapWidth - 4), Mathf.Max(0, mapHeight - 4));
            view.target.InitMovableItem(targetPos, gridEdge, mapWidth, mapHeight, view.gridParent, OnTargetPosChanged);

            view.stepSlider.maxValue = (float)gridCount;

            DoSearch();
        }

        protected void OnPlayerPosChanged()
        {
            DoSearch();
        }

        protected void OnTargetPosChanged()
        {
            DoSearch();
        }

        void OnStepSliderValueChanged(float value)
        {
            Step = (int)value;
        }

        void OnStepToggleChanged(bool isOn)
        {
            ShowStep = isOn;
        }

        void PathFindingOnClickGrid(object[] ps)
        {
            OnClickGrid((int)ps[0]);
        }

        protected virtual void OnClickGrid(int gridIndex)
        {
        }

        protected virtual void DoSearch()
        {
        }

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

        protected int GetRightwardGridIndex(int index)
        {
            if (index < 0 || index >= (mapWidth - 1) * mapHeight) return -1;
            return index + mapHeight;
        }

        protected int GetDownwardGridIndex(int index)
        {
            if (index < 0 || index >= mapWidth * mapHeight || mapHeight == 0 || index % mapHeight == 0) return -1;
            return index - 1;
        }

        protected int GetLefttwardGridIndex(int index)
        {
            if (index < mapHeight || index >= mapWidth * mapHeight) return -1;
            return index - mapHeight;
        }

        protected int GetUpwardGridIndex(int index)
        {
            if (index < 0 || index >= mapWidth * mapHeight || mapHeight == 0 || index % mapHeight == (mapHeight - 1)) return -1;
            return index + 1;
        }

        protected float GetArrowAngle(int curGrid, int cameFromGrid)
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

        protected void CalcNeighborIndexs(ref int[] neighbors, int curIndex)
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

        protected int Heuristic(int aIdx, int bIdx)
        {
            int aX = aIdx / mapHeight;
            int aY = aIdx % mapHeight;
            int bX = bIdx / mapHeight;
            int bY = bIdx % mapHeight;
            return Mathf.Abs(aX - bX) + Mathf.Abs(aY - bY);
        }

        protected int Cross(int curIdx, int targetIdx, int startIdx)
        {
            int curX = curIdx / mapHeight;
            int curY = curIdx % mapHeight;
            int targetX = targetIdx / mapHeight;
            int targetY = targetIdx % mapHeight;
            int startX = startIdx / mapHeight;
            int startY = startIdx % mapHeight;
            int x1 = curX - targetX;
            int y1 = curY - targetY;
            int x2 = startX - targetX;
            int y2 = startY - targetY;
            return Mathf.Abs(x1 * y2 - x2 * y1);
        }
        #endregion

        #region Grid pool

        Stack<PathFindingGridView> gridStack = new Stack<PathFindingGridView>();

        protected PathFindingGridView GetOneGrid()
        {
            var grid = view.gridPrefab.Spawn(view.gridParent);
            grid.gameObject.SetActive(true);
            grid.transform.localScale = Vector3.one;
            return grid;
        }

        protected void RecycleGrid(PathFindingGridView grid)
        {
            grid.Recycle();
        }
        #endregion

        protected virtual void UpdateGrids()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].SetGridState(curMapData.showArray[i], curMapData.IsBlock(i));
            }

            UpdateGridTexts();
            UpdatePaths();
        }

        protected List<Text> gridTextList = new List<Text>();
        protected virtual void UpdateGridTexts() 
        {
            
        }

        protected void ShowGridTexts(Dictionary<int, int> textDict)
        {
            int gridTextCount = 0;
            for (int i = 0; i < gridList.Count; i++)
            {
                if (textDict.ContainsKey(i))
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
                    gridTextList[gridTextCount - 1].text = textDict[i].ToString();
                }
            }

            while (gridTextCount < gridTextList.Count)
            {
                gridTextList[gridTextList.Count - 1].Recycle();
                gridTextList.RemoveAt(gridTextList.Count - 1);
            }
        }

        #region path

        protected List<int> pathList = new List<int>();           // 储存路径

        List<GameObject> pathGOList = new List<GameObject>();
        protected void UpdatePaths()
        {
            if (view == null) return;

            while (pathGOList.Count > pathList.Count)
            {
                pathGOList[pathGOList.Count - 1].Recycle();
                pathGOList.RemoveAt(pathGOList.Count - 1);
            }

            while (pathGOList.Count < pathList.Count)
            {
                GameObject path = view.pathPrefab.Spawn(view.pathParent);
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
                    pathGOList[i].transform.position = gridList[curGrid].transform.position;
                    float angle = GetArrowAngle(curGrid, cameFrom);
                    pathGOList[i].name = curGrid.ToString();
                    pathGOList[i].transform.localRotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }

        #endregion
    }
}