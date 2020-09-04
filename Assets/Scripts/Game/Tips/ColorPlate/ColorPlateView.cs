using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using UnityEngine.UI;

namespace SthGame
{
    public class ColorPlateView : UIBasePopupView
    {
        public RawImage rawImage;
        public Image nowColor;
        public Slider hueSlider;
        public Slider slider_R;
        public Slider slider_G;
        public Slider slider_B;
        public Slider slider_A;
        public Text valueTxt_R;
        public Text valueTxt_G;
        public Text valueTxt_B;
        public Text valueTxt_A;
    }
}