using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIElementBaseCtrl : UIChildController
    {
        UIElementBaseView view;

        protected string Desc { get; set; }

        public override void Init()
        {
            base.Init();

            view = uiNode as UIElementBaseView;
        }
    }
}

