using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class FunctionListItemData 
    {
        public string Desc { get; private set; }
        public int GoPos { get; private set; }
        public FunctionListItemData(string desc, int goPos)
        {
            Desc = desc;
            GoPos = goPos;
        }
    }

    public class FunctionListItemController : CircularListItemController
    {
        FunctionListItemView view;
        FunctionListItemData data;

        protected override string GetResourcePath()
        {
            return "Prefabs/FunctionListItem";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as FunctionListItemView;

            view.button.onClick.AddListener(() =>
            {
                if (data != null)
                { 
                    GUIManager.Instance.GoToPos(data.GoPos);
                }
            });
        }

        public override void SetListData(System.Object inData)
        {
            ListItemData itemData = inData as ListItemData;
            if (itemData.Data == null) return;
            data = (inData as ListItemData).Data as FunctionListItemData;

            view.text.text = data.Desc;
        }
    }
}
