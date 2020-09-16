using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            // tone
            view.tone_Dropdown.onValueChanged.AddListener(Tone_OnDropDownValueChanged);
            view.tone_Slider.onValueChanged.AddListener(Tone_OnSliderValueChanged);

            // hue
            view.hue_ColorBtn.onClick.AddListener(Hue_OnClickHueColorBtn);
            view.hue_RangeSlider.onValueChanged.AddListener(Hue_OnRangeSliderChanged);
            view.hue_HueSlider.onValueChanged.AddListener(Hue_OnHueSliderChanged);
            view.hue_SaturationSlider.onValueChanged.AddListener(Hue_OnSaturationSliderChanged);
            view.hue_ValueSlider.onValueChanged.AddListener(Hue_OnValueSliderChanged);

            // edgeDetection
            view.edgeDetection_Dropdown.onValueChanged.AddListener(EdgeDetection_OnDropDownValueChanged);
            view.edgeDetection_EdgeColBtn.onClick.AddListener(EdgeDetection_OnClickEdgeColorBtn);
            view.edgeDetection_EdgeWidthSlider.onValueChanged.AddListener(EdgeDetection_OnEdgeWidthSliderChanged);
            view.edgeDetection_BgToggle.onValueChanged.AddListener(EdgeDetection_OnBgToggleChanged);
            view.edgeDetection_BgAlphaSlider.onValueChanged.AddListener(EdgeDetection_OnBgAlphaSliderChanged);
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {
            OnClickMenuButton(EShaderDemoType.EdgeDetection);
        }

        private void OnClickMenuButton(EShaderDemoType type)
        {
            view.titleTxt.text = GetTitleTxt(type);

            view.demoObj_Tone.SetActive(type == EShaderDemoType.Tone);
            view.demoObj_Hue.SetActive(type == EShaderDemoType.Hue);
            view.demoObj_EdgeDetection.SetActive(type == EShaderDemoType.EdgeDetection);

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
            }
        }

        private string GetTitleTxt(EShaderDemoType type)
        {
            return type.ToString();
        }

        #region Tone
        private void Tone_InitEffect()
        {
            view.tone_Dropdown.value = (int)view.tone_Effect.effectMode;
            view.tone_Slider.value = view.tone_Effect.effectFactor;
        }
        private void Tone_OnDropDownValueChanged(int index)
        {
            view.tone_Effect.effectMode = (UIToneEffectMode)index;
        }

        private void Tone_OnSliderValueChanged(float value)
        {
            view.tone_Effect.effectFactor = value;
        }
        #endregion

        #region Hue
        private void Hue_InitEffect()
        {
            view.hue_ColorImg.color = view.hue_Effect.targetColor;
            view.hue_RangeSlider.value = view.hue_Effect.range;

            view.hue_HueSlider.value = view.hue_Effect.hue + 0.5f;
            view.hue_SaturationSlider.value = view.hue_Effect.saturation + 0.5f;
            view.hue_ValueSlider.value = view.hue_Effect.value + 0.5f;

            view.hue_RangeTxt.text = view.hue_RangeSlider.value.ToString("0.000");
            view.hue_HueTxt.text = view.hue_HueSlider.value.ToString("0.000");
            view.hue_SaturationTxt.text = view.hue_SaturationSlider.value.ToString("0.000");
            view.hue_ValueTxt.text = view.hue_ValueSlider.value.ToString("0.000");
        }

        private void Hue_OnClickHueColorBtn()
        {
            GUIManager.Instance.OpenColorPlate(view.hue_ColorImg.color, Hue_OnColorChanged, view.hue_ColorImg.transform);
        }

        private void Hue_OnColorChanged(Color color)
        {
            if (view && view.hue_ColorImg)
            {
                view.hue_ColorImg.color = color;
                view.hue_Effect.targetColor = color;
            }
        }

        private void Hue_OnRangeSliderChanged(float value)
        {
            view.hue_Effect.range = value;
            view.hue_RangeTxt.text = value.ToString("0.000");
        }

        private void Hue_OnHueSliderChanged(float value)
        {
            view.hue_Effect.hue = value - 0.5f;
            view.hue_HueTxt.text = view.hue_Effect.hue.ToString("0.000");
        }

        private void Hue_OnSaturationSliderChanged(float value)
        {
            view.hue_Effect.saturation = value - 0.5f;
            view.hue_SaturationTxt.text = view.hue_Effect.saturation.ToString("0.000");
        }

        private void Hue_OnValueSliderChanged(float value)
        {
            view.hue_Effect.value = value - 0.5f;
            view.hue_ValueTxt.text = view.hue_Effect.value.ToString("0.000");
        }
        #endregion

        #region Edge Detection
        private void EdgeDetection_InitEffect()
        {
            view.edgeDetection_Dropdown.value = (int)view.edgeDetection_Effect.edgeDetectionMode;
            view.edgeDetection_EdgeColImg.color = view.edgeDetection_Effect.edgeColor;
            view.edgeDetection_BgToggle.isOn = view.edgeDetection_Effect.bgToggle == 1;
            view.edgeDetection_BgAlphaSlider.gameObject.SetActive(view.edgeDetection_BgToggle.isOn);

            view.edgeDetection_EdgeWidthSlider.value = view.edgeDetection_Effect.edgeWidth / 2;
            view.edgeDetection_BgAlphaSlider.value = view.edgeDetection_Effect.bgAlpha;

            view.edgeDetection_EdgeWidthTxt.text = view.edgeDetection_Effect.edgeWidth.ToString("0.000");
            view.edgeDetection_BgFadeTxt.text = view.edgeDetection_Effect.bgAlpha.ToString("0.000");
        }

        private void EdgeDetection_OnDropDownValueChanged(int index)
        {
            view.edgeDetection_Effect.edgeDetectionMode = (UIEdgeDetectionMode)index;
        }

        private void EdgeDetection_OnClickEdgeColorBtn()
        {
            GUIManager.Instance.OpenColorPlate(view.edgeDetection_EdgeColImg.color, (color) =>
            {
                if (view && view.edgeDetection_EdgeColImg)
                {
                    view.edgeDetection_EdgeColImg.color = color;
                    view.edgeDetection_Effect.edgeColor = color;
                }
            }, view.edgeDetection_EdgeColImg.transform);
        }

        private void EdgeDetection_OnEdgeWidthSliderChanged(float value)
        {
            view.edgeDetection_Effect.edgeWidth = value * 2;
            view.edgeDetection_EdgeWidthTxt.text = view.edgeDetection_Effect.edgeWidth.ToString("0.000");
        }

        private void EdgeDetection_OnBgToggleChanged(bool isOn)
        {
            view.edgeDetection_Effect.bgToggle = isOn ? 1 : 0;
            view.edgeDetection_BgAlphaSlider.gameObject.SetActive(isOn);
        }

        private void EdgeDetection_OnBgAlphaSliderChanged(float value)
        {
            view.edgeDetection_Effect.bgAlpha = value;
            view.edgeDetection_BgFadeTxt.text = view.edgeDetection_Effect.bgAlpha.ToString("0.000");
        }
        #endregion
    }

    public enum EShaderDemoType
    {
        Tone,
        Hue,
        EdgeDetection,
    }
}