using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class UIRoot : MonoBehaviour
    {
        public Canvas[] LayerRoots = new Canvas[(int)UILayer._Count];
    }
}