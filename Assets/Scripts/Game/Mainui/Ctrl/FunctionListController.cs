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
            ListItemData data = new ListItemData() { Width = 380, Height = 400, Data = "listview示例" };
            horDataList.Add(data);
            //horDataList.Add(data);
            //horDataList.Add(data);
            //for (int i = 0; i < 10; i++)
            //{
            //    horDataList.Add(data);
            //}
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
