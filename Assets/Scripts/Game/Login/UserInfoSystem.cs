using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;

namespace SthGame
{
    public class UserInfoSystem : ClientSystem
    {
        UserInfoDataStore userInfoDataStore;

        public static UserInfoSystem Instance { get; private set; }

        public override void Init()
        {
            Instance = this;

            userInfoDataStore = DataStoreManager.Instance.FindOrBindDataStore<UserInfoDataStore>();
            GlobalEventSystem.Instance.Bind(EventId.onConnectNetwork, OnConnectNetwork);

            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSUserLoginRes, OnCSUserLoginRes);
            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSUserRegisterRes, OnCSUserRegisterRes);
            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSCreatePlayerRes, OnCSCreatePlayerRes);
            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSPlayerLoginRes, OnCSPlayerLoginRes);
        }

        public override void ShutDown()
        {
            GlobalEventSystem.Instance.UnBind(EventId.onConnectNetwork, OnConnectNetwork);

            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSUserLoginRes);
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSUserRegisterRes);
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSCreatePlayerRes);
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSPlayerLoginRes);
        }

        private void OnConnectNetwork(object[] ps)
        {
            if (!userInfoDataStore.IsLoginSuccess)
            {
                GUIManager.Instance.Open<LoginController>();
            }
        }

        private void OnCSUserLoginRes(string jsonStr)
        {
            CSUserLoginRes res = JsonMapper.ToObject<CSUserLoginRes>(jsonStr);
            userInfoDataStore.SetLoginRes(res);
        }

        private void OnCSUserRegisterRes(string jsonStr)
        {
            CSUserRegisterRes res = JsonMapper.ToObject<CSUserRegisterRes>(jsonStr);
            userInfoDataStore.SetRegisterRes(res);
        }

        private void OnCSCreatePlayerRes(string jsonStr)
        {
            CSCreatePlayerRes res = JsonMapper.ToObject<CSCreatePlayerRes>(jsonStr);
            if (res.CreateSuccess)
            {
                DoPlayerLogin();
            }
            else
            {
                GUIManager.Instance.OpenTipsView(res.Result);
            }
        }

        private void OnCSPlayerLoginRes(string jsonStr)
        {
            CSPlayerLoginRes res = JsonMapper.ToObject<CSPlayerLoginRes>(jsonStr);
            if (res.Success)
            {
                userInfoDataStore.LocalPlayerInfo = res.Player;
                GUIManager.Instance.CloseAllUI();
                GUIManager.Instance.Open<MainuiController>(uiLayer: UILayer.MainUI);
            }
            else
            {
                GUIManager.Instance.OpenTipsView(res.Result);
            }
        }

        private void DoPlayerLogin()
        {
            CSPlayerLoginReq req = new CSPlayerLoginReq();
            NetworkSystem.Instance.SendEvent(req);
        }
    }
}