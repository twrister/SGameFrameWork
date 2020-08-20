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
            horizontalListCtrl.InitList<FunctionListItemController>(view.horScrollRect, EDirection.Horizontal, 1);
            List<ListItemData> horDataList = new List<ListItemData>();

            var exampleList = new ListItemData() { Width = 380, Height = 400, Data = new FunctionListItemData("listview示例", () =>
            {
                //GUIManager.Instance.GoToPos(EGoToPosType.ExampleList);
                GUIManager.Instance.Open<ExampleListShowController>();
            }) };

            var shaderSample = new ListItemData()
            {
                Width = 380,
                Height = 400,
                Data = new FunctionListItemData("shader示例", () =>
                {
                    GUIManager.Instance.Open<ShaderDemoController>();
                })
            };

            horDataList.Add(exampleList);
            horDataList.Add(shaderSample);

            horizontalListCtrl.SetListData(horDataList);
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {

        }
    }
}
