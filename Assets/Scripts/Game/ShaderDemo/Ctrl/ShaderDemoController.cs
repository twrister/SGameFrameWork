using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SthGame
{
    public class ShaderDemoController : UIBaseController
    {
        ShaderDemoView view;
        EShaderDemoType _menuType;
        protected override string GetResourcePath()
        {
            return "Prefabs/ShaderDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as ShaderDemoView;

            view.closeBtn.onClick.AddListener(OnClickClose);

            // menu
            view.menuBtn_Tone.onClick.AddListener(() => OnClickMenuButton(EShaderDemoType.Tone));
            view.menuBtn_Hue.onClick.AddListener(() => OnClickMenuButton(EShaderDemoType.Hue));
            view.menuBtn_EdgeDetection.onClick.AddListener(() => OnClickMenuButton(EShaderDemoType.EdgeDetection));
            view.menuBtn_Blur.onClick.AddListener(() => OnClickMenuButton(EShaderDemoType.Blur));
            view.menuBtn_Flip.onClick.AddListener(() => OnClickMenuButton(EShaderDemoType.Flip));
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {
            OnClickMenuButton(EShaderDemoType.Tone);
        }

        private void OnClickMenuButton(EShaderDemoType type)
        {
            view.titleTxt.text = GetTitleTxt(type);

            view.tone_Effect.gameObject.SetActive(type == EShaderDemoType.Tone);
            view.hue_Effect.gameObject.SetActive(type == EShaderDemoType.Hue);
            view.edgeDetection_Effect.gameObject.SetActive(type == EShaderDemoType.EdgeDetection);
            view.blur_Effect.gameObject.SetActive(type == EShaderDemoType.Blur);
            view.flip_Effect.gameObject.SetActive(type == EShaderDemoType.Flip);

            HideAllUIElements();

            switch (type)
            {
                case EShaderDemoType.Tone:
                    Tone_InitEffect();
                    break;
                case EShaderDemoType.Hue:
                    Hue_InitEffect();
                    break;
                case EShaderDemoType.EdgeDetection:
                    EdgeDetection_InitEffect();
                    break;
                case EShaderDemoType.Blur:
                    Blur_InitEffect();
                    break;
                case EShaderDemoType.Flip:
                    Flip_InitEffect();
                    break;
            }
        }

        private string GetTitleTxt(EShaderDemoType type)
        {
            return type.ToString();
        }

        private void Tone_InitEffect()
        {
            ShowUIElementDropdown("Tone选项", typeof(UIToneEffectMode),
                (int)view.tone_Effect.effectMode).SetListener((index) =>
                {
                    view.tone_Effect.effectMode = (UIToneEffectMode)index;
                });

            ShowUIElementSlider("Factor", 0, 1, view.tone_Effect.effectFactor).SetListener((value) =>
            {
                view.tone_Effect.effectFactor = value;
            });
        }

        private void Hue_InitEffect()
        {
            ShowUIElementColorSet("边缘颜色", view.hue_Effect.targetColor).SetListener((color) =>
            {
                view.hue_Effect.targetColor = color;
            });
            ShowUIElementSlider("范围", 0, 1, view.hue_Effect.range).SetListener((value) =>
            {
                view.hue_Effect.range = value;
            });
            ShowUIElementSlider("Hue", 0, 1, view.hue_Effect.hue).SetListener((value) =>
            {
                view.hue_Effect.hue = value;
            });
            ShowUIElementSlider("Saturation", 0, 1, view.hue_Effect.saturation).SetListener((value) =>
            {
                view.hue_Effect.saturation = value;
            });
            ShowUIElementSlider("Value", 0, 1, view.hue_Effect.value).SetListener((value) =>
            {
                view.hue_Effect.value = value;
            });
        }

        private void EdgeDetection_InitEffect()
        {
            ShowUIElementDropdown("选项", typeof(UIEdgeDetectionMode),
                (int)view.edgeDetection_Effect.edgeDetectionMode).SetListener((index) =>
                {
                    view.edgeDetection_Effect.edgeDetectionMode = (UIEdgeDetectionMode)index;
                });
            ShowUIElementSlider("边缘宽度", 0, 2, view.edgeDetection_Effect.edgeWidth).SetListener((value) =>
            {
                view.edgeDetection_Effect.edgeWidth = value;
            });
            ShowUIElementColorSet("边缘颜色", view.edgeDetection_Effect.edgeColor).SetListener((color) =>
            {
                view.edgeDetection_Effect.edgeColor = color;
            });
            ShowUIElementToggle("显示背景", view.edgeDetection_Effect.bgToggle == 1).SetListener((isOn) =>
            {
                view.edgeDetection_Effect.bgToggle = isOn ? 1 : 0;
            });
            ShowUIElementSlider("背景透明度", 0, 1, view.edgeDetection_Effect.bgAlpha).SetListener((value) =>
            {
                view.edgeDetection_Effect.bgAlpha = value;
            });
        }

        private void Blur_InitEffect()
        {
            ShowUIElementDropdown("模糊算法", typeof(UIBlurMode), (int)view.blur_Effect.blurMode).SetListener(index =>
            {
                view.blur_Effect.blurMode = (UIBlurMode)index;
            });

            ShowUIElementSlider("Factor", 0, 1, view.blur_Effect.factor).SetListener((value) =>
            {
                view.blur_Effect.factor = value;
            });
        }

        private void Flip_InitEffect()
        {
            ShowUIElementToggle("Horizontal", view.flip_Effect.horizontal).SetListener(isOn =>
            {
                view.flip_Effect.horizontal = isOn;
            });

            ShowUIElementToggle("Vertical", view.flip_Effect.vertical).SetListener(isOn =>
            {
                view.flip_Effect.vertical = isOn;
            });
        }

        #region pool
        Dictionary<Type, Stack<UIElementBaseCtrl>> elementPoolDict = new Dictionary<Type, Stack<UIElementBaseCtrl>>();
        List<UIElementSliderController> uiSliderList = new List<UIElementSliderController>();
        List<UIElementDropdownController> uiDropdownList = new List<UIElementDropdownController>();
        List<UIElementColorSetController> uiColorSetList = new List<UIElementColorSetController>();
        List<UIElementToggleController> uiToggleList = new List<UIElementToggleController>();
        private void RecycleUIElement<T>(T element) where T : UIElementBaseCtrl
        {
            if (element != null)
            {
                element.UINode.SetParent(view.uiElementTempParent.transform);
                element.Reset();

                if (elementPoolDict.ContainsKey(typeof(T)))
                {
                    elementPoolDict[typeof(T)].Push(element);
                }
                else
                {
                    var stack = new Stack<UIElementBaseCtrl>();
                    stack.Push(element);
                    elementPoolDict.Add(typeof(T), stack);
                }
            }
        }

        private T GetUIElement<T>() where T : UIElementBaseCtrl
        {
            T element = null;

            Stack<UIElementBaseCtrl> stack;

            if (elementPoolDict.ContainsKey(typeof(T)))
            {
                stack = elementPoolDict[typeof(T)];
            }
            else
            {
                stack = new Stack<UIElementBaseCtrl>();
                elementPoolDict.Add(typeof(T), stack);
            }

            if (stack.Count == 0)
            {
                element = CreateChildController(typeof(T), view.uiElementParent) as T;
            }
            else
            {
                element = stack.Pop() as T;
                element.UINode.SetParent(view.uiElementParent.transform);
            }

            element.SetActive(true);
            return element;
        }

        private UIElementSliderController ShowUIElementSlider(string desc, float from, float to, float value)
        {
            var slider = GetUIElement<UIElementSliderController>();
            slider.InitBaseValue(desc, from, to, value);
            uiSliderList.Add(slider);
            return slider;
        }

        private UIElementDropdownController ShowUIElementDropdown(string desc, Type enumType, int index)
        {
            var dropdown = GetUIElement<UIElementDropdownController>();
            dropdown.InitBaseValue(desc, enumType, index);
            uiDropdownList.Add(dropdown);
            return dropdown;
        }

        private UIElementColorSetController ShowUIElementColorSet(string desc, Color defaultColor)
        {
            var colorSet = GetUIElement<UIElementColorSetController>();
            colorSet.InitBaseValue(desc, defaultColor);
            uiColorSetList.Add(colorSet);
            return colorSet;
        }

        private UIElementToggleController ShowUIElementToggle(string desc, bool isOn)
        {
            var toggle = GetUIElement<UIElementToggleController>();
            toggle.InitBaseValue(desc, isOn);
            uiToggleList.Add(toggle);
            return toggle;
        }

        private void HideAllUIElements()
        {
            for (int i = 0; i < uiSliderList.Count; i++)
                RecycleUIElement<UIElementSliderController>(uiSliderList[i]);

            for (int i = 0; i < uiDropdownList.Count; i++)
                RecycleUIElement<UIElementDropdownController>(uiDropdownList[i]);

            for (int i = 0; i < uiColorSetList.Count; i++)
                RecycleUIElement<UIElementColorSetController>(uiColorSetList[i]);

            for (int i = 0; i < uiToggleList.Count; i++)
                RecycleUIElement<UIElementToggleController>(uiToggleList[i]);

            uiSliderList.Clear();
            uiDropdownList.Clear();
            uiColorSetList.Clear();
            uiToggleList.Clear();
        }
        #endregion
    }

    public enum EShaderDemoType
    {
        Tone,
        Hue,
        EdgeDetection,
        Blur,
        Flip,
    }
}