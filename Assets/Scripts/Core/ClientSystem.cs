using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class ClientSystem
    {
        public ClientSystem()
        {
            Init();
        }

        public virtual void Init()
        {
        }

        public virtual void ShutDown()
        {
        }

        public virtual void ReLogin()
        {
        }

        public virtual void Tick(float deltaTime) { }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            if (null != GameRoot.Instance)
            {
                return GameRoot.Instance.StartCoroutine(routine);
            }
            return null;
        }
    }
}

