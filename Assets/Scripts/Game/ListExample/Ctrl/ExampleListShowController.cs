using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class ExampleListShowController : UIBaseController
    {
        ExampleListShowView view;
        CircularListController verticalListCtrl;
        CircularListController horizontalListCtrl;
        protected override string GetResourcePath()
        {
            return "Prefabs/ExampleListShow";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as ExampleListShowView;

            view.bgBtn.onClick.AddListener(OnClickClose);
            view.closeBtn.onClick.AddListener(OnClickClose);

            // Vertical
            verticalListCtrl = CreateChildController<CircularListController>(parent: view.ver_ScrollRect.viewport.gameObject);
            verticalListCtrl.InitList<ExampleListItemController>(view.ver_ScrollRect, EDirection.Vertical, 1);

            List<ListItemData> verDataList = new List<ListItemData>();
            for (int i = 0; i < 30; i++)
            {
                ListItemData data = new ListItemData() { Width = 400, Height = Random.Range(60, 150), Data = i.ToString() };
                verDataList.Add(data);
            }
            verticalListCtrl.SetListData(verDataList);

            view.ver_ScrollToBtn.onClick.AddListener(() =>
            {
                verticalListCtrl.ScrollTo(10);
            });
            view.ver_AddItemBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = 400, Height = Random.Range(60, 150), Data = "new" };
                verticalListCtrl.AddListItem(data, 10);
            });
            view.ver_AddItemEndBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = 400, Height = Random.Range(60, 150), Data = "new" };
                verticalListCtrl.AddListItem(data);
            });
            view.ver_UpdateItemBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = 400, Height = Random.Range(60, 150), Data = "new" };
                verticalListCtrl.UpdateItem(data, 10);
            });

            // Horizontal
            horizontalListCtrl = CreateChildController<CircularListController>(parent: view.hor_ScrollRect.viewport.gameObject);
            horizontalListCtrl.InitList<ExampleListItemController>(view.hor_ScrollRect, EDirection.Horizontal, 1);

            List<ListItemData> horDataList = new List<ListItemData>();
            for (int i = 0; i < 30; i++)
            {
                ListItemData data = new ListItemData() { Width = Random.Range(120, 160), Height = 300, Data = i.ToString() };
                horDataList.Add(data);
            }
            horizontalListCtrl.SetListData(horDataList);
            view.hor_ScrollToBtn.onClick.AddListener(() =>
            {
                horizontalListCtrl.ScrollTo(10);
            });
            view.hor_AddItemBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = Random.Range(120, 160), Height = 300, Data = "new" };
                horizontalListCtrl.AddListItem(data, 10);
            });
            view.hor_AddItemEndBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = Random.Range(120, 160), Height = 300, Data = "new" };
                horizontalListCtrl.AddListItem(data);
            });
            view.hor_UpdateItemBtn.onClick.AddListener(() =>
            {
                ListItemData data = new ListItemData() { Width = Random.Range(120, 160), Height = 300, Data = "new" };
                horizontalListCtrl.UpdateItem(data, 10);
            });
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
