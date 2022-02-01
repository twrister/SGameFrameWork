﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class FunctionListController : UIBaseController
    {
        FunctionListView view;
        CircularListController horizontalListCtrl;
        protected override string GetResourcePath()
        {
            return "Prefabs/FunctionListView";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as FunctionListView;

            view.closeBtn.onClick.AddListener(OnClickClose);
            view.closeBtn.gameObject.CreatePool();          // 临时用作对象池示例

            // Horizontal
            horizontalListCtrl = CreateChildController<CircularListController>(parent: view.horScrollRect.viewport.gameObject);
            horizontalListCtrl.InitList<FunctionListItemController>(view.horScrollRect, EDirection.Horizontal, 2);
            List<ListItemData> horDataList = new List<ListItemData>();

            var copyBufferDemo = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("copyBUffer demo", () =>
                {
                    string copyBuffer = UnityEngine.GUIUtility.systemCopyBuffer;
                    GUIManager.Instance.ShowFloatTips(copyBuffer);
                    Logger.Log("copyBuffer = {0}", copyBuffer);
                })
            };

            var exampleList = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("listview demo", () =>
                {
                    GUIManager.Instance.Open<ExampleListShowController>();
                })
            };

            var shaderSample = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("shader demo", () =>
                {
                    GUIManager.Instance.Open<ShaderDemoController>();
                })
            };

            var colorPlate = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("color plate", null, EFunctionItemType.colorPlate)
            };

            var bfsItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("BFS Path Finding", () =>
                {
                    GUIManager.Instance.Open<BFSPathFindingController>();
                })
            };

            var aStarItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("AStar Path Finding", () =>
                {
                    GUIManager.Instance.Open<AStarPathFindingController>();
                })
            };

            var noticeTipsItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("common tips", () =>
                {
                    GUIManager.Instance.OpenTipsView("普通弹窗的文字");
                })
            };

            var floatTipsItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("float tips", () =>
                {
                    float duration = Random.Range(0.5f, 1.5f);
                    string content = string.Format("上漂提示文字,持续{0:0.00}秒", duration);
                    GUIManager.Instance.ShowFloatTips(content, duration);
                })
            };


            var timerItem = new ListItemData()
            {
                Width = 250, Height = 300,
                Data = new FunctionListItemData("Start Timer", OnClickTimerDemo)
            };

            var redDotDemo = new ListItemData()
            {
                Width = 250, Height = 300,
                Data = new FunctionListItemData("red point demo", () => {
                    GUIManager.Instance.Open<RedPointDemoController>();
                },
                redPoint: ERedPointType.RedPointDemo)
            };

            var spawnTest = new ListItemData()
            {
                Width = 250, Height = 300,
                Data = new FunctionListItemData("pool spawn test", () =>
                {
                    objQueue.Enqueue(ObjectPool.Spawn(view.closeBtn.gameObject, this.view.transform, new Vector3(Random.Range(-100, 100), Random.Range(-100, 100))));
                })
            };

            var recycleTest = new ListItemData()
            {
                Width = 250, Height = 300,
                Data = new FunctionListItemData("pool recycle test", () =>
                {
                    if (objQueue.Count > 0)
                    {
                        ObjectPool.Recycle(objQueue.Dequeue());
                    }
                })
            };

            var poolInfo = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("debug pool info", () =>
                {
                    ObjectPool.DebugPoolInfo();
                })
            };

            var openQQ = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("call QQ", () =>
                {
                    Application.OpenURL("mqq://");
                    //var androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    //var androidObj = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
                    //androidObj.Call("DoActivieApp", "com.tencent.mobileqq");
                })
            };

            var mahjongChess = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("Mahjong Chess", () =>
                {
                    GUIManager.Instance.Open<MahjongChessController>();
                })
            };

            horDataList.Add(copyBufferDemo);
            horDataList.Add(exampleList);
            horDataList.Add(shaderSample);
            horDataList.Add(bfsItem);
            horDataList.Add(aStarItem);
            horDataList.Add(noticeTipsItem);
            horDataList.Add(colorPlate);
            horDataList.Add(floatTipsItem);
            horDataList.Add(timerItem);
            horDataList.Add(redDotDemo);
            horDataList.Add(spawnTest);
            horDataList.Add(recycleTest);
            horDataList.Add(poolInfo);
            horDataList.Add(openQQ);
            horDataList.Add(mahjongChess);

            horizontalListCtrl.SetListData(horDataList);
        }

        Queue<GameObject> objQueue = new Queue<GameObject>();

        Timer timer = null;
        private void OnClickTimerDemo()
        {
            if (timer != null)
            {
                GlobalTimerManager.Instance.ClearTimer(timer);
                Logger.Log("clear timer : {0}", timer.ToString());
            }
            else
            {
                Logger.Log("start timer");
                timer = GlobalTimerManager.Instance.StartTimer(1, true, () => {
                    Logger.Log("timer : {0}", timer.ToString());
                });
            }
        }

        private void OnClickClose()
        {
            Close();
        }

        public override void ShutDown()
        {
            base.ShutDown();

            view.closeBtn.gameObject.DestroyPooled();
        }
    }
}
