using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class BFSPathFindingView : PathFindingBaseView
    {
        public GameObject arrowPrefab;
        public GameObject arrowParent;

        public Toggle arrowToggle;
        public Toggle movementCostToggle;
    }
}