using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularScrollView;
using UnityEngine.Events;

namespace SthGame
{
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
                if (data != null )
                {
                    if (data.eFunctionItemType == EFunctionItemType.colorPlate)
                    {
                        ColorPlateCallback();
                    }
                    if (data.Callback != null)
                    {
                        data.Callback();
                    }
                }
            });
        }

        public override void SetListData(System.Object inData)
        {
            ListItemData itemData = inData as ListItemData;
            if (itemData.Data == null) return;
            data = (inData as ListItemData).Data as FunctionListItemData;

            RefreshView();
        }

        private void RefreshView()
        {
            if (data == null) return;

            view.titleTxt.text = "";
            view.colorObj.SetActive(false);
            switch (data.eFunctionItemType)
            {
                case EFunctionItemType.normal:
                    RefreshNormalView();
                    break;
                case EFunctionItemType.colorPlate:
                    RefreshColorView();
                    break;
            }
        }

        private void RefreshNormalView()
        {
            view.titleTxt.text = data.Desc;
        }

        private void RefreshColorView()
        {
            view.colorObj.SetActive(true);
            view.colTitleTxt.text = data.Desc;
            view.colorImg.color = Color.red;
            view.valuesTxt.text = string.Format("{0}\n{1}\n{2}", Color.red.r * 255, Color.red.g * 255, Color.red.b * 255);
        }

        private void ColorPlateCallback()
        {
            GUIManager.Instance.OpenColorPlate(view.colorImg.color, (color) => 
            {
                if (view && view.colorImg)
                {
                    view.colorImg.color = color;
                }
            });
        }
    }
}
