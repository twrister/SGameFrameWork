using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;
using UnityEngine.Events;

namespace SthGame
{
    public class FunctionListItemData 
    {
        public string Desc { get; private set; }
        public EGoToPosType GoPos { get; private set; }
        public UnityAction Callback { get; private set; }
        public FunctionListItemData(string desc, UnityAction callback = null)
        {
            Desc = desc;
            Callback = callback;
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
                if (data != null && data.Callback != null)
                {
                    data.Callback();
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
