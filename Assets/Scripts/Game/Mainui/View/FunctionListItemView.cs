using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class FunctionListItemView : UIBaseView
    {
        public Button button;
        public Text titleTxt;

        public GameObject colorObj;
        public Text colTitleTxt;
        public Button colorBtn;
        public Image colorImg;
        public Text valuesTxt;

        public RedPointView redPoint;
    }
}