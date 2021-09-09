using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        protected override string GetResourcePath()
        {
            return "";
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

            GlobalEventSystem.Instance.Bind(EventId.aStarOnClickGrid, PathFindingOnClickGrid);
        }

        public override void ShutDown()
        {
            base.ShutDown();

            GlobalEventSystem.Instance.UnBindAll(EventId.aStarOnClickGrid);
        }

        protected void InitGrids()
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
            Vector2Int targetPos = new Vector2Int(Mathf.Max(0, mapWidth - 4), Mathf.Max(0, mapHeight - 4));
            view.target.InitMovableItem(targetPos, gridEdge, mapWidth, mapHeight, view.gridParent.transform, OnTargetPosChanged);

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

        protected virtual void PathFindingOnClickGrid(object[] ps)
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
        #endregion

        #region Grid pool

        Stack<PathFindingGridView> gridStack = new Stack<PathFindingGridView>();

        protected PathFindingGridView GetOneGrid()
        {
            PathFindingGridView grid = null;

            if (gridStack.Count == 0)
            {
                view.gridPrefab.gameObject.SetActive(true);
                grid = GameObject.Instantiate<PathFindingGridView>(view.gridPrefab, view.gridParent.transform);
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

        protected void RecycleGrid(PathFindingGridView grid)
        {
            if (grid != null)
            {
                grid.Reset();
                gridStack.Push(grid);
            }
        }
        #endregion

        Dictionary<int, Stack<PoolObject>> poolDict = new Dictionary<int, Stack<PoolObject>>();

        protected T GetOneInPool<T>(T prefab, Transform trans) where T : UnityEngine.Object
        {
            T item = default(T);
            int poolKey = prefab.GetInstanceID();
            if (!poolDict.ContainsKey(poolKey))
            {
                poolDict[poolKey] = new Stack<PoolObject>();
            }

            if (poolDict[poolKey].Count == 0)
            {
                PoolObject poolObj = new PoolObject(poolKey);
                poolObj.obj = GameObject.Instantiate<T>(prefab, trans);
            }
            else
            {
                item = poolDict[poolKey].Pop().obj as T;
            }

            return item;
        }

        protected void RecycleToPool(PoolObject obj)
        {
            if (poolDict.ContainsKey(obj.PoolKey))
            {
                poolDict[obj.PoolKey].Push(obj);
            }
        }

        protected virtual void UpdateGrids()
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].SetGridState(curMapData.showArray[i], curMapData.IsBlock(i));
            }
        }
    }

    public class PoolObject
    {
        public int PoolKey { get; private set; }
        public UnityEngine.Object obj;

        public PoolObject(int key)
        {
            PoolKey = key;
        }
    }
}