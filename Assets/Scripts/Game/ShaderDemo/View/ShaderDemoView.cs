using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class ShaderDemoView : UIBaseView
    {
        public Button closeBtn;
        public Text titleTxt;

        // menu
        public Button menuBtn_Tone;
        public Button menuBtn_Hue;
        // demo obj
        public GameObject demoObj_Tone;
        public GameObject demoObj_Hue;

        // tone
        public UIToneEffect tone_Effect;
        public Dropdown tone_Dropdown;
        public Slider tone_Slider;

        // hue
        public UIHSVModifier hue_Effect;
        public Button hue_ColorBtn;
        public Image hue_ColorImg;
        public Slider hue_RangeSlider;
        public Slider hue_HueSlider;
        public Slider hue_SaturationSlider;
        public Slider hue_ValueSlider;
        public Text hue_RangeTxt;
        public Text hue_HueTxt;
        public Text hue_SaturationTxt;
        public Text hue_ValueTxt;
    }
}