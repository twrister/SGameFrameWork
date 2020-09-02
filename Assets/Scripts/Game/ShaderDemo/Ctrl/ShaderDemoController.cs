using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class ShaderDemoController : UIBaseController
    {
        ShaderDemoView view;
        protected override string GetResourcePath()
        {
            return "Prefabs/ShaderDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as ShaderDemoView;

            view.closeBtn.onClick.AddListener(OnClickClose);

            // tone
            view.tone_Dropdown.onValueChanged.AddListener(Tone_OnDropDownValueChange);
            view.tone_Slider.onValueChanged.AddListener(Tone_OnSliderValueChange);
        }

        private void OnClickClose()
        {
            Close();
        }

        protected override void OpenCallBack()
        {
            InitToneEffect();
        }

        #region Tone
        private void InitToneEffect()
        {
            view.tone_Dropdown.value = (int)view.tone_Effect.effectMode;
            view.tone_Slider.value = view.tone_Effect.effectFactor;
        }
        private void Tone_OnDropDownValueChange(int index)
        {
            view.tone_Effect.effectMode = (EffectMode)index;
        }

        private void Tone_OnSliderValueChange(float value)
        {
            view.tone_Effect.effectFactor = value;
        }
        #endregion
    }
}