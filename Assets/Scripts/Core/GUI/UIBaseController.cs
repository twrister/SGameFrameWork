using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIBaseController : UIBaseContainer
    {
        public bool IsOpen { get; private set; }

        public virtual void Open()
        {
            if (HasUIView() && !IsOpen)
            {
                SetActive(true);
            }
        }

        public virtual void Close()
        {
            Hide();
            GUIManager.Instance.RemoveUIController(this);
        }

        public virtual void Hide()
        {
            SetActive(false);
        }

        protected virtual void OpenCallBack() { }

        protected virtual void HideCallBack() { }

        public virtual void SetActive(bool active)
        {
            if (HasUIView())
            {
                if (IsOpen != active)
                {
                    uiNode.SetActive(active);
                    IsOpen = active;

                    if (active)
                    {
                        OpenCallBack();
                    }
                    else
                    {
                        HideCallBack();
                    }
                }
            }
        }
    }
}