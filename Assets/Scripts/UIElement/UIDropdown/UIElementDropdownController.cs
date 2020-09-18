using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace SthGame
{
    public class UIElementDropdownController : UIElementBaseCtrl
    {
        UIElementDropdownView view;
        UnityAction<int> dropdownDelegate;
        Type _enumType;

        protected override string GetResourcePath()
        {
            return "Prefabs/UIElement/UIElement_Dropdown";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as UIElementDropdownView;

            view.dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int value)
        {
            if (dropdownDelegate != null)
            {
                dropdownDelegate(value);
            }
        }

        public UIElementDropdownController SetListener(UnityAction<int> action)
        {
            dropdownDelegate = action;
            return this;
        }

        public override void Reset()
        {
            base.Reset();

            dropdownDelegate = null;
        }

        public void InitBaseValue(string desc, Type enumType, int index)
        {
            SetDescText(desc);
            _enumType = enumType;

            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            var enumNameList = new List<string>(System.Enum.GetNames(enumType));
            view.dropdown.ClearOptions();
            view.dropdown.AddOptions(enumNameList);
            view.dropdown.value = index;
        }
    }
}
