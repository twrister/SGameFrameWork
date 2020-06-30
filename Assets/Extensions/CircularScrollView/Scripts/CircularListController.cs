using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SthGame;
using UnityEngine.UI;

namespace CircularScrollView
{
    public class ListItemData
    {
        public object Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector3 Pos { get; set; }
        public CircularListItemController Controller { get; set; }
    }
    public class CircularListController : UIChildController
    {
        CircularListView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/CircularListView";
        }

        public override void Init()
        {
            base.Init();

            view = uiNode as CircularListView;
        }

        public override void ShutDown()
        {
            base.ShutDown();
        }

        private bool isInit = false;
        private int columns = 1;
        private EDirection eDirection = EDirection.Horizontal;
        List<ListItemData> dataList;
        private ScrollRect scrollRect;
        private System.Type childType;

        public bool IsHorizontal { get { return eDirection == EDirection.Horizontal; } }

        public void InitList<T>(
            ScrollRect inScrollRect,
            EDirection eDir = EDirection.Horizontal,
            int inColumns = 1)
        {
            childType = typeof(T);
            scrollRect = inScrollRect;
            scrollRect.content = this.Rect;
            columns = inColumns;
            eDirection = eDir;
            ResetAnchor(view.contentRectRrans);

            scrollRect.onValueChanged.AddListener(delegate (Vector2 value) { ScrollRectListener(value); });

            isInit = true;
        }

        public void SetListData(List<ListItemData> datas)
        //where T : UIChildController, new()
        {
            dataList = datas;

            // -> 计算content大小位置
            int totalSize = 0;      // content total size
            int cellMaxSize = 0;
            int tmpCellLength = 0;
            int maxCellLength = 0;
            int itemPosX = 0;
            int itemPosY = 0;

            for (int i = 0; i < dataList.Count; i++)
            {
                int cellSize = IsHorizontal ? dataList[i].Width : dataList[i].Height;
                cellMaxSize = Mathf.Max(cellMaxSize, cellSize);
                tmpCellLength += IsHorizontal ? dataList[i].Height : dataList[i].Width;
                if (i % columns == columns - 1) // 行中末item
                {
                    totalSize += cellMaxSize;
                    maxCellLength = Mathf.Max(maxCellLength, tmpCellLength);
                    cellMaxSize = 0;
                    tmpCellLength = 0;
                }

                // 计算item位置
                if (i % columns == 0)       // 首item
                    itemPosX = 0;

                dataList[i].Pos = IsHorizontal ?
                    new Vector3(itemPosY, itemPosX) : new Vector3(itemPosX, -itemPosY);

                itemPosX += IsHorizontal ? dataList[i].Width : dataList[i].Height;
                itemPosY = totalSize;
            }

            totalSize = Mathf.Max(totalSize, IsHorizontal ?
                (int)Mathf.Abs(GetContentRect().x) : (int)Mathf.Abs(GetContentRect().y));

            view.contentRectRrans.sizeDelta = IsHorizontal ?
                new Vector2(totalSize, maxCellLength) : new Vector2(maxCellLength, totalSize);


            // -> 创建item
            UpdateList();
        }

        public void UpdateList()
        {
            if (dataList == null) return;

            for (int i = 0; i < dataList.Count; i++)
            {
                ListItemData tempData = dataList[i];
                if (IsOverRange(tempData))       // 超过的回收
                {
                    SetPoolChildController(tempData.Controller);
                    tempData.Controller = null;
                }
                else
                {
                    if (tempData.Controller == null)
                    {
                        Logger.Log("New {2}: \tx = {0}\ty = {1}\theight = {3}", tempData.Pos.x, tempData.Pos.y, i, tempData.Height);
                        tempData.Controller = GetPoolChildController();
                        ResetAnchor(tempData.Controller.Rect);
                        tempData.Controller.SetAnchoredPosition(tempData.Pos);
                        tempData.Controller.SetRectSize(tempData.Width, tempData.Height);
                        tempData.Controller.Name = i.ToString();

                        tempData.Controller.SetListData(tempData);
                    }
                }
            }
        }

        public void ScrollTo(int index)
        {
            if (index < 0 || index >= dataList.Count)
            {
                Logger.Error("index out of range");
                return;
            }

            float value = 0;

            for (int i = 0; i < index; i++)
            {
                value += IsHorizontal ? -dataList[i].Width : dataList[i].Height;
            }
            scrollRect.content.anchoredPosition = IsHorizontal ? new Vector2(value, 0) : new Vector2(0, value);
        }

        public void AddListItem(ListItemData data, int index = -1)
        {
            if (index == -1) index = Mathf.Max(0, dataList.Count);

            if (index > 0)
            {
                ListItemData lastData = dataList[index - 1];
                data.Pos = lastData.Pos + (IsHorizontal ? Vector3.right * lastData.Width : Vector3.down * lastData.Height);
            }
            dataList.Insert(index, data);

            float oriSize = IsHorizontal ? view.contentRectRrans.sizeDelta.x : view.contentRectRrans.sizeDelta.y;
            float totalSize = IsHorizontal ? oriSize + data.Width : oriSize + data.Height;
            float cellLength = IsHorizontal ? view.contentRectRrans.sizeDelta.y : view.contentRectRrans.sizeDelta.x;
            totalSize = Mathf.Max(totalSize, IsHorizontal ?
                (int)Mathf.Abs(GetContentRect().x) : (int)Mathf.Abs(GetContentRect().y));

            view.contentRectRrans.sizeDelta = IsHorizontal ?
                new Vector2(totalSize, cellLength) : new Vector2(cellLength, totalSize);

            for (int i = index; i < dataList.Count; i++)
            {
                ListItemData tempData = dataList[i];
                if (i > index)
                    tempData.Pos += IsHorizontal ? Vector3.right * data.Width : Vector3.down * data.Height;

                if (IsOverRange(tempData))       // 超过的回收
                {
                    SetPoolChildController(tempData.Controller);
                    tempData.Controller = null;
                }
                else
                {
                    if (tempData.Controller == null)
                    {
                        Logger.Log("New {2}: \tx = {0}\ty = {1}\theight = {3}", tempData.Pos.x, tempData.Pos.y, i, tempData.Height);
                        tempData.Controller = GetPoolChildController();
                    }
                    ResetAnchor(tempData.Controller.Rect);
                    tempData.Controller.SetAnchoredPosition(tempData.Pos);
                    tempData.Controller.SetRectSize(tempData.Width, tempData.Height);
                    tempData.Controller.Name = i.ToString();

                    tempData.Controller.SetListData(tempData);
                }
            }
        }

        public void UpdateItem(ListItemData data, int index)
        {
            if (index < 0 || index >= dataList.Count)
            {
                Logger.Error("UpdateItem - index out of range");
                return;
            }

            ListItemData oriData = dataList[index];
            float oriSize = IsHorizontal ? view.contentRectRrans.sizeDelta.x : view.contentRectRrans.sizeDelta.y;
            float deltaSize = IsHorizontal ? -oriData.Width + data.Width : -oriData.Height + data.Height;
            float totalSize = oriSize + deltaSize;
            float cellLength = IsHorizontal ? view.contentRectRrans.sizeDelta.y : view.contentRectRrans.sizeDelta.x;
            totalSize = Mathf.Max(totalSize, IsHorizontal ?
                (int)Mathf.Abs(GetContentRect().x) : (int)Mathf.Abs(GetContentRect().y));
            view.contentRectRrans.sizeDelta = IsHorizontal ?
                new Vector2(totalSize, cellLength) : new Vector2(cellLength, totalSize);

            dataList[index].Width = data.Width;
            dataList[index].Height = data.Height;

            for (int i = index; i < dataList.Count; i++)
            {
                ListItemData tempData = dataList[i];
                if (i > index)
                    tempData.Pos += IsHorizontal ? Vector3.right * deltaSize : Vector3.down * deltaSize;

                if (IsOverRange(tempData))       // 超过的回收
                {
                    SetPoolChildController(tempData.Controller);
                    tempData.Controller = null;
                }
                else
                {
                    if (tempData.Controller == null)
                    {
                        Logger.Log("New {2}: \tx = {0}\ty = {1}\theight = {3}", tempData.Pos.x, tempData.Pos.y, i, tempData.Height);
                        tempData.Controller = GetPoolChildController();
                    }
                    ResetAnchor(tempData.Controller.Rect);
                    tempData.Controller.SetAnchoredPosition(tempData.Pos);
                    tempData.Controller.SetRectSize(tempData.Width, tempData.Height);
                    tempData.Controller.Name = i.ToString();

                    tempData.Controller.SetListData(tempData);
                }
            }
        }

        #region 内部方法
        private bool IsOverRange(ListItemData data)
        {
            if (IsHorizontal)
            {
                float contentPos = view.contentRectRrans.anchoredPosition.x;
                float itemPosMin = data.Pos.x + contentPos;
                float itemPosMax = itemPosMin + data.Width;
                float contentMaxPos = GetContentRect().xMax;
                return itemPosMin > contentMaxPos || itemPosMax < 0f;
            }
            else
            {
                float contentPos = view.contentRectRrans.anchoredPosition.y;
                float itemPosMax = data.Pos.y + contentPos;
                float itemPosMin = itemPosMax - data.Height;
                float contentMinPos = GetContentRect().yMin;
                return itemPosMin > 0f || itemPosMax < contentMinPos;
            }
            
        }

        private Rect GetContentRect()
        {
            return scrollRect.viewport.rect;
        }

        private void ResetAnchor(RectTransform rectTrans)
        {
            rectTrans.pivot = Vector2.up;
            rectTrans.anchorMin = Vector2.up;
            rectTrans.anchorMax = Vector2.up;
            //rectTrans.anchorMin = eDirection == EDirection.Horizontal ? Vector2.zero : Vector2.up; 
            //rectTrans.anchorMax = eDirection == EDirection.Horizontal ? Vector2.up : Vector2.one;
        }

        private void ScrollRectListener(Vector2 value)
        {
            UpdateList();
        }

        #endregion

        #region Pool
        protected Stack<CircularListItemController> childPool = new Stack<CircularListItemController>();
        protected virtual CircularListItemController GetPoolChildController()
        {
            CircularListItemController controller = null;
            if (childPool.Count > 0)
            {
                controller = childPool.Pop();
            }

            if (controller == null)
            {
                controller = CreateChildController(childType, childControllerList.Count - 1) as CircularListItemController;
            }
            controller.SetActive(true);

            return controller;
        }

        protected virtual void SetPoolChildController(CircularListItemController childController)
        {
            if (childController != null)
            {
                childPool.Push(childController);
                childController.SetActive(false);
            }
        }
        #endregion
    }
}
