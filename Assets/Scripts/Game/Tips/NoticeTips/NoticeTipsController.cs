using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SthGame
{
    public class NoticeTipsController : UIBasePopupController
    {
        UnityAction btnDelegate1;
        UnityAction btnDelegate2;

        NoticeTipsView view;

        string tipsContext = "";
        string buttonText1 = "";
        string buttonText2 = "";

        protected override string GetResourcePath()
        {
            return "Prefabs/NoticeView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as NoticeTipsView;

            view.button1.onClick.AddListener(OnClickButton1);
            view.button2.onClick.AddListener(OnClickButton2);
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            FlushView();
        }

        public void SetData(
            string tips, 
            string btnText1, UnityAction btnDel1 = null,
            string btnText2 = "", UnityAction btnDel2 = null)
        {
            tipsContext = tips;
            buttonText1 = btnText1;
            buttonText2 = btnText2;
            btnDelegate1 = btnDel1;
            btnDelegate2 = btnDel2;
        }

        private void FlushView()
        {
            view.noticeText.text = tipsContext;
            view.buttonText1.text = buttonText1;
            view.buttonText2.text = buttonText2;

            view.button2.gameObject.SetActive(!string.IsNullOrEmpty(buttonText2));
        }

        private void OnClickButton2()
        {
            Close();
            if (btnDelegate2 != null)
            {
                btnDelegate2();
            }
        }

        private void OnClickButton1()
        {
            Close();
            if (btnDelegate1 != null)
            {
                btnDelegate1();
            }
        }

        private void ReConnect()
        {
            GameRoot.Instance.DoReLogin();
        }
    }
}