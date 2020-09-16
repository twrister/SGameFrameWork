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
        public Button menuBtn_EdgeDetection;
        // demo obj
        public GameObject demoObj_Tone;
        public GameObject demoObj_Hue;
        public GameObject demoObj_EdgeDetection;

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

        // edge detection
        public UIEdgeDetection edgeDetection_Effect;
        public Dropdown edgeDetection_Dropdown;
        public Button edgeDetection_EdgeColBtn;
        public Image edgeDetection_EdgeColImg;
        public Slider edgeDetection_EdgeWidthSlider;
        public Text edgeDetection_EdgeWidthTxt;
        public Toggle edgeDetection_BgToggle;
        public Slider edgeDetection_BgAlphaSlider;
        public Text edgeDetection_BgFadeTxt;
    }
}