using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SthGame
{
    public enum UILayer
    {
        Game,
        MainUI,
        Common,
        Popup,
        Notice,
        _Count,
    }

    public class UITools
    {
        //public static UIBuilder Instance = new UIBuilder();

        private static Transform UIRoot = GameObject.Find("GameRoot/UIRoot").transform;
        private static UIRoot Root = UIRoot.GetComponent<UIRoot>();

        public static T CreateUIController<T>(int userContext = 0) 
            where T : UIBaseController, new()
        {
            T controller = new T();
            string ControllerID = GetControllerKey<T>(userContext);
            controller.SetControllerID(ControllerID);

            return controller;
        }

        public static T CreateUIChildController<T>(int userContext = 0)
            where T : UIChildController, new()
        {
            T controller = new T();
            string ControllerID = GetControllerKey<T>(userContext);
            controller.SetControllerID(ControllerID);

            return controller;
        }

        public static UIChildController CreateUIChildController(System.Type controllerType, int userContext = 0)
        {
            UIChildController controller = Activator.CreateInstance(controllerType) as UIChildController;
            string ControllerID = GetControllerKey(controllerType.Name, userContext);
            controller.SetControllerID(ControllerID);

            return controller;
        }

        public static string GetControllerKey<T>(int userContext)
        {
            return GetControllerKey(typeof(T).Name, userContext);
        }

        public static string GetControllerKey(string controllerTypeFullName, int userContext)
        {
            return string.Format("{0}-{1}", controllerTypeFullName, userContext.ToString());
        }

        // parent为空时，生成到对应的layer下
        public static T AttachUIView<T>(
            string abPath,
            UIBaseContainer owner,
            GameObject parent = null,
            UILayer attachLayer = UILayer.Common,
            Vector3 localPosition = new Vector3(),
            bool isFullScreen = true
            ) where T : Component
        {
            T result = null;

            if (parent == null)
            {
                parent = UITools.GetLayerRoot(attachLayer).gameObject;
            }

            if (parent != null)
            {
                result = InstanceAssetGameObject<T>(abPath, parent, isFullScreen);
                if (result != null)
                {
                    UIBaseView uiView = result as UIBaseView;
                    if (uiView != null)
                    {
                        uiView.SetOwner(owner);
                    }
                    result.name = owner.ControllerId;
                    result.gameObject.transform.localPosition = localPosition;
                }
            }

            return result;
        }

        private static T InstanceAssetGameObject<T>(
            string abPath,
            GameObject parent = null,
            bool isFullScreen = true
            ) where T : Component
        {
            T result = null;

            //var abArray = (abPath ?? "").Split('#');
            //if (abArray != null && abArray.Length == 2)
            if (!string.IsNullOrEmpty(abPath))
            {
                //GameObject obj = AssetBundleLoader.LoadAssetBundle(abArray[0], abArray[1]);
                GameObject obj = Resources.Load<GameObject>(abPath);

                if (obj != null)
                {
                    obj = GameObject.Instantiate(obj) as GameObject;
                    if (obj != null)
                    {
                        if (parent != null)
                        {
                            //GameTools.AddChild(UIRoot);
                            
                            obj.transform.SetParent(parent.transform, false);
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localRotation = Quaternion.identity;
                            obj.transform.localScale = Vector3.one;

                            if (isFullScreen)
                            {
                                RectTransform rt = obj.transform as RectTransform;
                                rt.anchorMin = Vector2.zero;
                                rt.anchorMax = Vector2.one;
                                rt.sizeDelta = Vector2.zero;
                            }
                        }

                        result = obj.GetComponent<T>();
                        if (result == null)
                        {
                            Logger.Error(string.Format("fail to find conponent {0}", typeof(T)));
                        }
                    }
                }
            }
            else
            {
                Logger.Error(string.Format("abPath is error : {0}", abPath));
            }

            return result;
        }

        public static Canvas GetLayerRoot(UILayer layer)
        {
            Canvas canvas = Root.LayerRoots[(int)layer];
            if (canvas == null)
            {
                Logger.Error(string.Format("canvas {0} is null", layer));
            }

            return canvas;
        }
    }
}
