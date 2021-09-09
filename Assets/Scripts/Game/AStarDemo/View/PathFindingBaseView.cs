using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class PathFindingBaseView : UIBaseView
    {
        public Button closeButton;

        public GameObject gridParent;
        public PathFindingGridView gridPrefab;

        public GameObject pathParent;
        public GameObject pathPrefab;

        public PathFindingMovableItem player;
        public PathFindingMovableItem target;

        public Toggle stepToggle;
        public Slider stepSlider;
    }
}