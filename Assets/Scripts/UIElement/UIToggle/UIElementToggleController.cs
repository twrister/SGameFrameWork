using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace SthGame
{
    public class UIElementToggleController : UIElementBaseCtrl
    {
        UIElementToggleView view;
        UnityAction<bool> toggleDelegate;

        protected override string GetResourcePath()
        {
            return "Prefabs/UIElement/UIElement_Toggle";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as UIElementToggleView;

            view.toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (toggleDelegate != null)
            {
                toggleDelegate(isOn);
            }
        }

        public UIElementToggleController SetListener(UnityAction<bool> action)
        {
            toggleDelegate = action;
            return this;
        }

        public override void Reset()
        {
            base.Reset();

            toggleDelegate = null;
        }

        public void InitBaseValue(string desc, bool isOn)
        {
            SetDescText(desc);
            view.toggle.isOn = isOn;
        }
    }
}
