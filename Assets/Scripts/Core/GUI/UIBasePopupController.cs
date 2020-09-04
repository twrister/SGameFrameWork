using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace SthGame
{
    public abstract class UIBasePopupController : UIBaseController
    {
        UIBasePopupView view;

        public override void Init()
        {
            base.Init();

            view = UINode as UIBasePopupView;

            if (view.bgBtn) view.bgBtn.onClick.AddListener(OnClickClose);
            if (view.closeBtn) view.closeBtn.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Close();
        }
    }
}