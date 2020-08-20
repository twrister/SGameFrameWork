using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class ShaderDemoController : UIBaseController
    {
        ShaderDemoView view;
        protected override string GetResourcePath()
        {
            return "Prefabs/ShaderDemo";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as ShaderDemoView;

            view.closeBtn.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Close();
        }
    }
}