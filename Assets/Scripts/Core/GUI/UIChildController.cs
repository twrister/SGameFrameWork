using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIChildController : UIBaseContainer
    {
        public string Name
        {
            get { return uiNode.name; }
            set { uiNode.name = value; }
        }
        public virtual void SetActive(bool active)
        {
            if (HasUIView())
            {
                uiNode.SetActive(active);
            }
        }

        public virtual void SetAnchoredPosition(Vector3 pos)
        {
            if (HasUIView())
            {
                (uiNode.chcheTransform as RectTransform).anchoredPosition = pos;
            }
        }

        public virtual void SetRectSize(int width, int height)
        {
            if (HasUIView())
            {
                (uiNode.chcheTransform as RectTransform).sizeDelta = new Vector2(width, height);
            }
        }
    }
}
