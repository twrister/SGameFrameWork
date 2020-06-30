using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SthGame;
using UnityEngine.UI;

namespace CircularScrollView
{
    public abstract class CircularListItemController : UIChildController
    {
        public virtual void SetListData(System.Object data)
        {
        }
    }
}