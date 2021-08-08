using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{

    public class BaseScene
    {
        private SceneInfo sceneInfo;
        public virtual void Init()
        {
            GUIManager.Instance.Open<MainuiController>();
        }

        public virtual void ShutDown()
        {
        }

        public BaseScene(SceneInfo info)
        {
            sceneInfo = info;
            Init();
        }

        public string SceneName { get { return sceneInfo.SceneName; } }
    }
}
