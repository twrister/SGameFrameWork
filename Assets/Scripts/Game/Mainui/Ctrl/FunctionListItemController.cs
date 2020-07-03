using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class FunctionListItemController : CircularListItemController
    {
        FunctionListItemView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/FunctionListItem";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as FunctionListItemView;

            //view.button.onClick.AddListener(() => {
            //    GUIManager.Instance.Open<ExampleListShowController>();
            //});
        }

        public override void SetListData(System.Object inData)
        {
            ListItemData data = inData as ListItemData;
            view.text.text = data.Data.ToString();
        }
    }
}
