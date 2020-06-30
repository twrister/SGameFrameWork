using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SthGame;
using UnityEngine.UI;

namespace SthGame
{
    public class PacManFrontEndView : UIBaseView
    {
        public Button closeBtn;
        public Button refreshBtn;
        public GameObject gridRootObj;
        public PacManPlayer player;

        public Text scoreText;
        public Text stepText;
        public Text descText;
        public Text calcDescText;

        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;
    }
}
