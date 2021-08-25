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

            if (view.redPoint != null)
            {
                view.redPoint.InitRedPointType(data.redPointType);
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
            SetColorPlate(Color.red);
        }

        private void ColorPlateCallback()
        {
            GUIManager.Instance.OpenColorPlate(view.colorImg.color, SetColorPlate, view.button.transform);
        }

        private void SetColorPlate(Color color)
        {
            if (view && view.colorImg)
            {
                view.colorImg.color = color;
                view.valuesTxt.text = string.Format("{0:0}\n{1:0}\n{2:0}", color.r * 255, color.g * 255, color.b * 255);
            }
        }
    }
}
