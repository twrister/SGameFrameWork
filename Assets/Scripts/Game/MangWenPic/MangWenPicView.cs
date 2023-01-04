using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class MangWenPicView : UIBaseView
    {
        public RawImage m_SourceRawImg;
        public RawImage m_TranferRawImg;
        public Text m_PicTxt;
        public Button m_RefreshBtn;
        public Button m_CloseBtn;

        public int m_PixelSize = 1;
        public float m_Threshold = 0.5f;
    }
}