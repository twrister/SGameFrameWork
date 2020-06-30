using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIBaseContainer
    {
        protected List<UIChildController> childControllerList = new List<UIChildController>();
        protected abstract string GetResourcePath();
        public virtual void Init() { }
        public virtual void ShutDown()
        {
            for (int i = 0; i < childControllerList.Count; i++)
            {
                childControllerList[i].ShutDown();
            }
            uiNode.ShutDown();
        }

        protected UIBaseView uiNode;
        public UIBaseView UINode { get { return uiNode; } }
        public RectTransform Rect { get { return HasUIView() ? uiNode.chcheTransform as RectTransform : null; } }
        public bool HasUIView() { return UINode != null; }

        public string ControllerId { get; protected set; }
        public void SetControllerID(string id) { ControllerId = id; }

        public GameObject CreateUIView(
            int context = 0,
            GameObject parent = null,
            UILayer uiLayer = UILayer.Common,
            Vector3 localPosition = new Vector3(),
            bool isFullScreen = true
            )
        {
            string resPath = GetResourcePath();
            if (!string.IsNullOrEmpty(resPath))
            {
                uiNode = UITools.AttachUIView<UIBaseView>(resPath, this, parent, uiLayer, localPosition, isFullScreen);
                if (uiNode != null)
                {
                    return uiNode.gameObject;
                }
            }

            return null;
        }

        public T CreateChildController<T>(
            int context = 0,
            GameObject parent = null,
            Vector3 localPosition = new Vector3()
            ) where T : UIChildController, new()
        {
            T childController = UITools.CreateUIChildController<T>(context);
            if (childController != null)
            {
                if (!childController.HasUIView())
                {
                    if (parent == null)
                    {
                        parent = this.UINode.chcheGameObject;
                    }

                    GameObject viewObj = childController.CreateUIView(context, parent, localPosition: localPosition);
                }

                if (childController != null)
                {
                    childController.Init();
                }

                childControllerList.Add(childController);
            }
            return childController;
        }

        public UIChildController CreateChildController(
            System.Type controllerType,
            int context = 0,
            GameObject parent = null,
            Vector3 localPosition = new Vector3())
        {
            UIChildController childController = UITools.CreateUIChildController(controllerType, context);
            if (childController != null)
            {
                if (!childController.HasUIView())
                {
                    if (parent == null)
                    {
                        parent = this.UINode.chcheGameObject;
                    }

                    GameObject viewObj = childController.CreateUIView(context, parent, localPosition: localPosition);
                }
                if (childController != null)
                {
                    childController.Init();
                }
                childControllerList.Add(childController);
            }
            return childController;
        }
    }
}
