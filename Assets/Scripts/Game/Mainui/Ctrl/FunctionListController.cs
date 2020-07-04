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

            var ExampleList = new ListItemData() { Width = 380, Height = 400, Data = new FunctionListItemData("listview示例", 10001)};

            horDataList.Add(ExampleList);
            horDataList.Add(ExampleList);

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
