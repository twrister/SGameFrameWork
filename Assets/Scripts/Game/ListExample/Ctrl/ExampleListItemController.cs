using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;

namespace SthGame
{
    public class ExampleListItemController : CircularListItemController
    {
        ExampleListItemView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/ExampleListItem";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as ExampleListItemView;
        }

        public override void SetListData(System.Object inData)
        {
            //if (inData == null) return;

            ListItemData data = inData as ListItemData;
            view.text.text = string.Format("{0}\nWidth:\t{1}\nHeight:\t{2}", data.Data, data.Width, data.Height);
        }
    }
}
