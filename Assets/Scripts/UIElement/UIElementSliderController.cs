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
            if (sliderDelegate != null) sliderDelegate(value);
        }

        public void SetListener(UnityAction<float> action)
        {
            sliderDelegate = action;
        }
    }
}
