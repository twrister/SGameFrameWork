using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class PathFindingBaseView : UIBaseView
    {
        public Button closeButton;

        public Transform gridParent;
        public PathFindingGridView gridPrefab;

        public Transform pathParent;
        public GameObject pathPrefab;

        public Transform gridTextParent;
        public Text gridTextPrefab;

        public PathFindingMovableItem player;
        public PathFindingMovableItem target;

        public Toggle stepToggle;
        public Slider stepSlider;
        public Toggle earlyExitToggle;
    }
}