using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SthGame
{
    public abstract class UIBaseContainer
    {
        protected List<UIChildController> childControllerList = new List<UIChildController>();
        protected abstract string GetResourcePath();
        public virtual void Init() { }
        public virtual void ShutDown()
        {
            childControllerCountDict.Clear();
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

        Dictionary<Type, int> childControllerCountDict = new Dictionary<Type, int>();
        protected void AddElementCount<T>()
        {
            AddElementCount(typeof(T));
        }
        protected void AddElementCount(Type type)
        {
            if (childControllerCountDict.ContainsKey(type))
                childControllerCountDict[type]++;
            else
                childControllerCountDict.Add(type, 1);
        }
        protected int GetElementCount<T>()
        {
            return GetElementCount(typeof(T));
        }

        protected int GetElementCount(Type type)
        {
            if (childControllerCountDict.ContainsKey(type))
                return childControllerCountDict[type];
            return 0;
        }

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
            GameObject parent = null,
            Vector3 localPosition = new Vector3()
            ) where T : UIChildController, new()
        {
            int context = GetElementCount<T>();
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
                AddElementCount<T>();
                childControllerList.Add(childController);
            }
            return childController;
        }

        public UIChildController CreateChildController(
            System.Type controllerType,
            GameObject parent = null,
            Vector3 localPosition = new Vector3())
        {
            int context = GetElementCount(controllerType);
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
                AddElementCount(controllerType);
                childControllerList.Add(childController);
            }
            return childController;
        }
    }
}
