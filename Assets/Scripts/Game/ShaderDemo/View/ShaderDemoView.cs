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

        // tone
        public GameObject tone_Obj;
        public UIEffect tone_Effect;
        public Dropdown tone_Dropdown;
        public Slider tone_Slider;
    }
}