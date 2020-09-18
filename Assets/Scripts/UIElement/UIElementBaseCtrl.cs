using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIElementBaseCtrl : UIChildController
    {
        UIElementBaseView view;

        public override void Init()
        {
            base.Init();

            view = uiNode as UIElementBaseView;
        }

        public virtual void Reset()
        {
            SetDescText("");
            this.SetActive(false);
        }

        protected void SetDescText(string desc)
        {
            view.descText.text = desc;
        }
    }
}

