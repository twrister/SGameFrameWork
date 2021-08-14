using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SthGame
{
    public class GUIManager : ClientSystem
    {
        public static GUIManager Instance { get; private set; }

        private List<UIBaseController> uiControllerList = new List<UIBaseController>();
        FloatTipsController floatTipsController;

        public override void Init()
        {
            Instance = this;
        }

        public T Open<T>(
            int context = 0,
            GameObject parent = null,
            UILayer uiLayer = UILayer.Common,
            Vector3 localPosition = new Vector3(),
            bool isFullScreen = true,
            bool openImmediately = true
            ) where T : UIBaseController, new()
        {
            string controllerId = UITools.GetControllerKey<T>(context);
            T controller = FindUIController(controllerId) as T;
            if (controller == null)
            {
                controller = CreateUIController<T>(context, parent, uiLayer, localPosition, isFullScreen);
            }

            if (controller != null)
            {
                controller.Init();
                if (openImmediately)
                {
                    controller.Open();
                }
            }
            return controller;
        }

        private T CreateUIController<T>(
            int context = 0,
            GameObject parent = null,
            UILayer uiLayer = UILayer.Common,
            Vector3 localPosition = new Vector3(),
            bool isFullScreen = true
            ) where T : UIBaseController, new()
        {
            T controller = UITools.CreateUIController<T>();

            if (controller != null)
            {
                // createUIView
                if (!controller.HasUIView())
                {
                    GameObject viewObj = controller.CreateUIView(context, parent, uiLayer, localPosition, isFullScreen);
                }
                controller.SetActive(false);
                uiControllerList.Add(controller);
            }

            return controller;
        }

        private UIBaseController FindUIController(string controllerId)
        {
            UIBaseController result = null;
            for (int i = 0; i < uiControllerList.Count; i++)
            {
                if (controllerId == uiControllerList[i].ControllerId)
                {
                    return uiControllerList[i];
                }
            }
            return result;
        }

        public void RemoveUIController(UIBaseController controller)
        {
            RemoveUIControllerById(controller.ControllerId);
        }

        void RemoveUIControllerById(string controllerId)
        {
            UIBaseController controller = FindUIController(controllerId);
            if (controller != null)
            {
                uiControllerList.Remove(controller);
                controller.ShutDown();
            }
        }

        public void CloseAllUI()
        {
            for (int i = uiControllerList.Count - 1; i >= 0; i--)
            {
                uiControllerList[i].Close();
            }
            uiControllerList.Clear();
        }

        public override void ShutDown()
        {
        }

        public override void ReLogin()
        {
            CloseAllUI();
        }

        #region Tips View

        public void OpenTipsView(
            string tipsContext = "notice",
            string confirmText = "确定",
            UnityAction btnDelegate1 = null,
            string btnText2 = "",
            UnityAction btnDelegate2 = null)
        {
            var noticeTipsCtrl = Open<NoticeTipsController>(uiLayer: UILayer.Popup, openImmediately: false);

            noticeTipsCtrl.SetData(tipsContext, confirmText, btnDelegate1, btnText2, btnDelegate2);
            noticeTipsCtrl.Open();
        }

        public void OpenHeadFrameChoose(
            int defaultIdx = 0,
            HeadFrameChooseController.ChoosedDelegate choosedDel = null)
        {
            var headFrameCtrl = Open<HeadFrameChooseController>(uiLayer: UILayer.Popup, openImmediately: false);
            headFrameCtrl.SetData(defaultIdx, choosedDel);
            headFrameCtrl.Open();
        }

        public void OpenColorPlate(
            Color inColor,
            UnityAction<Color> callback,
            Transform attachTrans)
        {
            var colorPlate = Open<ColorPlateController>(uiLayer: UILayer.Popup, openImmediately: false);

            colorPlate.SetColor(inColor, callback, attachTrans);
            colorPlate.Open();
        }

        public void ShowFloatTips(string content, float duration = 1.5f)
        {
            if (floatTipsController == null)
            {
                floatTipsController = Open<FloatTipsController>(uiLayer: UILayer.Notice);
            }

            if (floatTipsController != null)
            {
                floatTipsController.ShowFloatTips(content, duration);
            }
        }

        #endregion
    }
}
