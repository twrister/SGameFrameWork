using System.Collections;
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

            // Horizontal
            horizontalListCtrl = CreateChildController<CircularListController>(parent: view.horScrollRect.viewport.gameObject);
            horizontalListCtrl.InitList<FunctionListItemController>(view.horScrollRect, EDirection.Horizontal, 2);
            List<ListItemData> horDataList = new List<ListItemData>();

            var exampleList = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("listview示例", () =>
                {
                    GUIManager.Instance.Open<ExampleListShowController>();
                })
            };

            var shaderSample = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("shader示例", () =>
                {
                    GUIManager.Instance.Open<ShaderDemoController>();
                })
            };

            var colorPlate = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("颜料板", null, EFunctionItemType.colorPlate)
            };

            var aStarItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("AStar", () =>
                {
                    //GUIManager.Instance.Open<ShaderDemoController>();
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("AStarDemo");
                    //GUIManager.Instance.CloseAllUI();
                    //GameSceneManager.Instance.LoadScene("AStarDemo");
                })
            };

            var noticeTipsItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("普通弹窗", () =>
                {
                    GUIManager.Instance.OpenTipsView("普通弹窗的文字");
                })
            };

            var floatTipsItem = new ListItemData()
            {
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("上漂提示", () =>
                {
                    float duration = Random.Range(1f, 3f);
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
                Width = 250,
                Height = 300,
                Data = new FunctionListItemData("红点", () => {
                    GUIManager.Instance.Open<RedPointDemoController>();
                },
                redPoint: ERedPointType.RedPointDemo)
            };

            horDataList.Add(exampleList);
            horDataList.Add(shaderSample);
            horDataList.Add(colorPlate);
            horDataList.Add(aStarItem);
            horDataList.Add(noticeTipsItem);
            horDataList.Add(floatTipsItem);
            horDataList.Add(timerItem);
            horDataList.Add(redDotDemo);

            horizontalListCtrl.SetListData(horDataList);
        }

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

        }
    }
}
