using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class RedPointView : MonoBehaviour, IRedPointDelegate
    {
        public ERedPointType type;
        public GameObject redPointGO;
        public Text redPointNumText;

        public void OnTipsChanged(ERedPointType redPointType, int number)
        {
            if (redPointGO != null) redPointGO.SetActive(number > 0);
            if (redPointNumText != null)
            {
                redPointNumText.text = number > 99 ? "99" : number > 1 ? number.ToString() : "";
            }
        }

        void Awake()
        {
            InitRedPoint();
        }

        private void InitRedPoint()
        {
            if (redPointGO != null) redPointGO.SetActive(false);
            RedPointManager.Instance.RegisterRedPointDelegate(type, this);
            RedPointManager.Instance.SendRedPointNotify(type);
        }

        public void InitRedPointType(ERedPointType inType)
        {
            if (inType != ERedPointType.None)
            {
                type = inType;
                InitRedPoint();
            }
        }

        void Destroy()
        {
            RedPointManager.Instance.UnRegisterRedPointDelegate(type, this);
        }
    }
}
