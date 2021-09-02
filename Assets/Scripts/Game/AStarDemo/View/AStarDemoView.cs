using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class AStarDemoView : UIBaseView
    {
        public Button closeButton;

        public GameObject gridParent;
        public AStarGridView gridPrefab;

        public AStarMovableItem player;
        public AStarMovableItem target;

        public Slider stepSlider;
    }
}