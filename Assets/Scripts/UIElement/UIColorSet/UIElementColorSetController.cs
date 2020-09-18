using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace SthGame
{
    public class UIElementColorSetController : UIElementBaseCtrl
    {
        UIElementColorSetView view;
        UnityAction<Color> colorSetDelegate;

        protected override string GetResourcePath()
        {
            return "Prefabs/UIElement/UIElement_ColorSet";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as UIElementColorSetView;

            view.colorButton.onClick.AddListener(OnClickColorButton);
        }

        private void OnClickColorButton()
        {
            GUIManager.Instance.OpenColorPlate(view.colorImage.color, OnColorChanged, view.colorImage.transform);
        }

        private void OnColorChanged(Color color)
        {
            UpdateColorShow(color);
            if (colorSetDelegate != null)
            {
                colorSetDelegate(color);
            }
        }

        public UIElementColorSetController SetListener(UnityAction<Color> action)
        {
            colorSetDelegate = action;
            return this;
        }

        public override void Reset()
        {
            base.Reset();

            colorSetDelegate = null;
        }

        public void InitBaseValue(string desc, Color defaultColor)
        {
            SetDescText(desc);
            UpdateColorShow(defaultColor);
        }

        private void UpdateColorShow(Color color)
        {
            view.colorImage.color = new Color(color.r, color.g, color.b);
            view.alphaSlider.value = color.a;
        }
    }
}
