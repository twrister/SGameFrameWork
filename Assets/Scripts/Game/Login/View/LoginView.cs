using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class LoginView : UIBaseView
    {
        public GameObject loginObj;
        public GameObject registerObj;
        public GameObject createPlayerObj;

        public InputField loginAccountInput;
        public InputField loginPasswordInput;
        public Button loginBtn;
        public Toggle rememberPasswordToggle;
        public Button goRegisterBtn;

        public InputField registerAccountInput;
        public InputField registerPasswordInput1;
        public InputField registerPasswordInput2;
        public Button registerBtn;
        public Button goLoginBtn;

        public InputField createPlayerInputField;
        public Button createPlayerBtn;
        public Image headFrameImage;
        public Button headFrameButton;

        public Button offlineButton;
    }
}