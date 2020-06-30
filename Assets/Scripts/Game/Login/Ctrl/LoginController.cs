using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;

namespace SthGame
{
    public class LoginController : UIBaseController, DataStoreSubscriber
    {
        LoginView view;
        bool isLoginMode { get; set; }
        UserInfoDataStore userInfoDataStore;
        bool isCreatePlayerMode = false;
        int defaultHeadFrameIdx = 0;

        protected override string GetResourcePath()
        {
            return "Prefabs/LoginView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as LoginView;

            view.rememberPasswordToggle.onValueChanged.AddListener(OnRememberPasswordToggleChanged);
            view.loginBtn.onClick.AddListener(OnClickLogin);
            view.goRegisterBtn.onClick.AddListener(OnClickGoRegister);
            view.registerBtn.onClick.AddListener(OnClickRegister);
            view.goLoginBtn.onClick.AddListener(OnClickGoLogin);
            view.createPlayerBtn.onClick.AddListener(OnClickCreatePlayer);
            view.headFrameButton.onClick.AddListener(OnClickHeadFrame);
            view.offlineButton.onClick.AddListener(OnClickOffline);

            userInfoDataStore = DataStoreManager.Instance.FindOrBindDataStore<UserInfoDataStore>();
            userInfoDataStore.RegisterSubscriber(this);

            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardEnter, OnClickKeyboardEnter);

            isLoginMode = true;
        }

        public override void ShutDown()
        {
            base.ShutDown();

            userInfoDataStore.UnRegisterSubscriber(this);

            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardEnter, OnClickKeyboardEnter);
        }

        private void OnRememberPasswordToggleChanged(bool isOn)
        {
            PlayerPrefs.SetInt("IS_SAVE_LOGIN_PASSWORD", isOn ? 1 : 0);
            if (!isOn)
            {
                PlayerPrefs.SetString("LOGIN_PASSWORD_KEY", "");
            }
        }

        private void OnClickLogin()
        {
            string inputAccount = view.loginAccountInput.text;
            string inputPassword = view.loginPasswordInput.text;
            string result = "";
            bool success = CheckLogin(inputAccount, inputPassword, out result);

            if (success)
            {
                DoLogin(inputAccount, inputPassword);
            }
            else
            {
                GUIManager.Instance.OpenTipsView(result);
            }
        }

        private bool CheckLogin(string account, string password, out string result)
        {
            result = "";
            if (string.IsNullOrEmpty(account))
            {
                result = "请输入用户名";
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                result = "请输入密码";
                return false;
            }
            if (!StringTools.IsValidAccountInput(account))
            {
                result = "账号需3位或以上英文或数字组合";
                return false;
            }
            if (!StringTools.IsValidAccountInput(password))
            {
                result = "密码需3位或以上英文或数字组合";
                return false;
            }
            return true;
        }

        private void DoLogin(string account, string password)
        {
            CSUserLoginReq req = new CSUserLoginReq();
            req.Account = account;
            req.Password = password;
            NetworkSystem.Instance.SendEvent(req);
        }

        private void OnClickRegister()
        {
            string inputAccount = view.registerAccountInput.text;
            string inputPassword1 = view.registerPasswordInput1.text;
            string inputPassword2 = view.registerPasswordInput2.text;
            string result = "";
            bool success = CheckRegister(inputAccount, inputPassword1, inputPassword2, out result);

            if (success)
            {
                DoRegister(inputAccount, inputPassword1);
            }
            else
            {
                GUIManager.Instance.OpenTipsView(result);
            }
        }

        private void OnClickCreatePlayer()
        {
            string createPlayerName = view.createPlayerInputField.text.Trim();
            if (string.IsNullOrEmpty(createPlayerName))
            {
                GUIManager.Instance.OpenTipsView("昵称不能为空");
            }
            else
            {
                DoCreatePlayer(createPlayerName);
            }
        }

        private void DoCreatePlayer(string name)
        {
            CSCreatePlayerReq req = new CSCreatePlayerReq();
            req.NickName = name;
            req.HeadFrameId = defaultHeadFrameIdx;
            NetworkSystem.Instance.SendEvent(req);
        }

        private bool CheckRegister(string account, string password1, string password2, out string result)
        {
            result = "";
            if (string.IsNullOrEmpty(account))
            {
                result = "请输入用户名";
                return false;
            }
            if (string.IsNullOrEmpty(password1))
            {
                result = "请输入密码";
                return false;
            }
            if (string.IsNullOrEmpty(password2))
            {
                result = "请输入确认密码";
                return false;
            }
            if (!StringTools.IsValidAccountInput(account))
            {
                result = "账号需3位或以上英文或数字组合";
                return false;
            }
            if (password1 != password2)
            {
                result = "两次输入密码不一致";
                return false;
            }
            if (!StringTools.IsValidAccountInput(password1))
            {
                result = "密码需3位或以上英文或数字组合";
                return false;
            }
            return true;
        }

        private void DoRegister(string account, string password)
        {
            CSUserRegisterReq req = new CSUserRegisterReq();
            req.Account = account;
            req.Password = password;
            NetworkSystem.Instance.SendEvent(req);
        }

        private void OnClickGoLogin()
        {
            isLoginMode = true;
            FlushView();
        }
        private void OnClickGoRegister()
        {
            isLoginMode = false;
            FlushView();
        }

        protected override void OpenCallBack()
        {
            InitView();
            FlushView();
        }

        private void InitView()
        {
            bool isOn = PlayerPrefs.GetInt("IS_SAVE_LOGIN_PASSWORD") == 1;
            view.rememberPasswordToggle.isOn = isOn;

            string inputAccount = PlayerPrefs.GetString("LOGIN_ACCOUNT_KEY");
            view.loginAccountInput.text = inputAccount;
            if (isOn)
            {
                string inputPassword = PlayerPrefs.GetString("LOGIN_PASSWORD_KEY");
                view.loginPasswordInput.text = inputPassword;
            }
        }

        private void FlushView()
        {
            view.loginObj.SetActive(isLoginMode && !isCreatePlayerMode);
            view.registerObj.SetActive(!isLoginMode && !isCreatePlayerMode);

            view.createPlayerObj.SetActive(isCreatePlayerMode);
            if (isCreatePlayerMode)
            {
                view.headFrameImage.LoadSprite("HeadAtlas", string.Format("head_{0}", defaultHeadFrameIdx));
            }
        }

        public void NotifyDataStoreUpdated(DataStore sourceDataStore, int index)
        {
            if (index == (int)UserInfoDataStore.UpdateType.LoginRes)
            {
                ProcessLoginRes();
            }
            else if (index == (int)UserInfoDataStore.UpdateType.RegisterRes)
            {
                ProcessRegisterRes();
            }
        }

        private void ProcessLoginRes()
        {
            CSUserLoginRes res = userInfoDataStore.LoginRes;

            if (res.LoginSuccess && res.HasPlayer)
            {
                DoPlayerLogin();
            }
            else
            {
                isCreatePlayerMode = true;
                FlushView();
            }

            if (res.LoginSuccess)
            {
                SaveLocalAccountAndPassword();
            }
        }

        private void DoPlayerLogin()
        {
            CSPlayerLoginReq req = new CSPlayerLoginReq();
            NetworkSystem.Instance.SendEvent(req);
        }

        private void SaveLocalAccountAndPassword()
        {
            string inputAccount = view.loginAccountInput.text;
            PlayerPrefs.SetString("LOGIN_ACCOUNT_KEY", inputAccount);

            if (view.rememberPasswordToggle.isOn)
            {
                string inputPassword = view.loginPasswordInput.text;
                PlayerPrefs.SetString("LOGIN_PASSWORD_KEY", inputPassword);
            }
        }

        private void ProcessRegisterRes()
        {
            CSUserRegisterRes res = userInfoDataStore.RegisterRes;
            if (res.RegisterSuccess)
            {
                GUIManager.Instance.OpenTipsView("注册成功", btnDelegate1: ()=> {
                    OnClickGoLogin();
                });
            }
            else
            {
                GUIManager.Instance.OpenTipsView(res.Result);
            }
        }

        private void ClearLoginInput()
        {
            view.loginAccountInput.text = "";
            view.loginPasswordInput.text = "";
        }

        private void OnClickHeadFrame()
        {
            GUIManager.Instance.OpenHeadFrameChoose(defaultHeadFrameIdx, choosedDel: 
                (idx) => {
                    defaultHeadFrameIdx = idx;
                    view.headFrameImage.LoadSprite("HeadAtlas", string.Format("head_{0}", idx));
                });
        }

        private void OnClickOffline()
        {
            GUIManager.Instance.CloseAllUI();
            GUIManager.Instance.Open<MainuiController>(uiLayer: UILayer.MainUI);
        }

        private void OnClickKeyboardEnter(object[] ps)
        {
            if (isCreatePlayerMode)
            {
                OnClickCreatePlayer();
            }
            else if (isLoginMode)
            {
                OnClickLogin();
            }
            else
            {
                OnClickRegister();
            }
        }
    }
}