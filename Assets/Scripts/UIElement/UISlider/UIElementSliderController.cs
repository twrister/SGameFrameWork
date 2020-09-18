using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SthGame
{
    public class UIElementSliderController : UIElementBaseCtrl
    {
        UIElementSliderView view;
        UnityAction<float> sliderDelegate;
        float _from = 0;
        float _to = 1;

        protected override string GetResourcePath()
        {
            return "Prefabs/UIElement/UIElement_Slider";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as UIElementSliderView;

            view.slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            if (sliderDelegate != null)
            {
                value = Interpolation(value);
                sliderDelegate(value);
                UpdateValueText(value);
            }
        }

        public UIElementSliderController SetListener(UnityAction<float> action)
        {
            sliderDelegate = action;
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            sliderDelegate = null;
        }

        public void InitBaseValue(string desc, float from, float to, float value)
        {
            SetDescText(desc);
            _from = from;
            _to = to;
            view.slider.value = (value - from) / (to - from);
            UpdateValueText(value);
        }

        private float Interpolation(float progress)
        {
            float value = Mathf.Lerp(_from, _to, progress);
            return value;
        }

        private void UpdateValueText(float value)
        {
            view.valueText.text = value.ToString("0.000");
        }
    }
}
