using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SthGame
{
    public class WorldSessionView : UIBaseView
    {
        public GridLayoutGroup chatMemberGrid;
        public VerticalLayoutGroup chatMessageGroup;
        public Text onlineText;
        public ScrollRect messageScrollRect;
    }
}