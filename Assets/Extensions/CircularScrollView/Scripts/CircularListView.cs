using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SthGame;
using UnityEngine.UI;

namespace CircularScrollView
{
    public enum EDirection
    {
        Horizontal,
        Vertical,
    }

    public class CircularListView : UIBaseView
    {
        public RectTransform contentRectRrans;
        public Vector2 GetContentRectSize()
        {
            return contentRectRrans.sizeDelta;
        }
    }

}
