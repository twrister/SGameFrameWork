using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public abstract class UIBaseView : MonoBehaviour
    {
        //UIBaseController owner;
        UIBaseContainer owner;

        public void SetActive(bool active)
        {
            chcheGameObject.SetActive(active);
        }

        protected GameObject go;
        public GameObject chcheGameObject { get { if (go == null) go = gameObject; return go; } }
        protected Transform trans;
        public Transform chcheTransform { get { if (trans == null) trans = transform; return trans; } }

        public virtual void SetOwner(UIBaseContainer InOwner)
        {
            owner = InOwner;
        }

        public virtual void ShutDown()
        {
            GameObject.Destroy(gameObject);
        }
    }
}