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

            // tone
            view.tone_Dropdown.onValueChanged.AddListener(Tone_OnDropDownValueChanged);
            view.tone_Slider.onValueChanged.AddListener(Tone_OnSliderValueChanged);

            // hue
            view.hue_ColorBtn.onClick.AddListener(Hue_OnClickHueColorBtn);
            view.hue_RangeSlider.onValueChanged.AddListener(Hue_OnRangeSliderChanged);
            view.hue_HueSlider.onValueChanged.AddListener(Hue_OnHueSliderChanged);
            view.hue_SaturationSlider.onValueChanged.AddListener(Hue_OnSaturationSliderChanged);
            view.hue_ValueSlider.onValueChanged.AddListener(Hue_OnValueSliderChanged);
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {
            OnClickMenuButton(EShaderDemoType.Hue);
        }

        private void OnClickMenuButton(EShaderDemoType type)
        {
            view.titleTxt.text = GetTitleTxt(type);

            view.demoObj_Tone.SetActive(type == EShaderDemoType.Tone);
            view.demoObj_Hue.SetActive(type == EShaderDemoType.Hue);

            switch (type)
            {
                case EShaderDemoType.Tone:
                    Tone_InitEffect();
                    break;
                case EShaderDemoType.Hue:
                    Hue_InitEffect();
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
            view.tone_Effect.effectMode = (EffectMode)index;
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
            view.hue_HueSlider.value = view.hue_Effect.hue;
            view.hue_SaturationSlider.value = view.hue_Effect.saturation;
            view.hue_ValueSlider.value = view.hue_Effect.value;

            view.hue_RangeTxt.text = view.hue_RangeSlider.value.ToString("0.000");
            view.hue_HueTxt.text = view.hue_HueSlider.value.ToString("0.000");
            view.hue_SaturationTxt.text = view.hue_SaturationSlider.value.ToString("0.000");
            view.hue_ValueTxt.text = view.hue_ValueSlider.value.ToString("0.000");
        }

        private void Hue_OnClickHueColorBtn()
        {
            GUIManager.Instance.OpenColorPlate(view.hue_ColorImg.color, Hue_OnColorPlateClose, view.hue_ColorImg.transform);
        }

        private void Hue_OnColorPlateClose(Color color)
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
            //view.hue_HueSlider.value = value;
            view.hue_HueTxt.text = value.ToString("0.000");
        }

        private void Hue_OnSaturationSliderChanged(float value)
        {
            //view.hue_SaturationSlider.value = value;
            view.hue_SaturationTxt.text = value.ToString("0.000");
        }

        private void Hue_OnValueSliderChanged(float value)
        {
            //view.hue_ValueSlider.value = value;
            view.hue_ValueTxt.text = value.ToString("0.000");
        }
        #endregion
    }

    public enum EShaderDemoType
    {
        Tone,
        Hue,
    }
}