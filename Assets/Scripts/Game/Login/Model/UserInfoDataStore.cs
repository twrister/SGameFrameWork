using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

namespace SthGame
{
    public class UserInfoDataStore : DataStore
    {
        public enum UpdateType
        {
            LoginRes,
            RegisterRes,
            PlayerInfoChanged,
        }

        public UserInfoDataStore()
            : base(DataStoreType.UserInfo)
        {
        }

        CSUserLoginRes loginRes;
        CSUserLoginRes emptyLoginRes = new CSUserLoginRes();

        public CSUserLoginRes LoginRes { get { return loginRes ?? emptyLoginRes; } }
        public bool IsLoginSuccess { get { return LoginRes.LoginSuccess; } }

        private PlayerInfo localPlayerInfo;
        public PlayerInfo LocalPlayerInfo
        {
            get { return localPlayerInfo; }
            set
            {
                localPlayerInfo = value;
                RefreshSubscribers((int)UpdateType.PlayerInfoChanged);
            }
        }

        public void SetLoginRes(CSUserLoginRes res)
        {
            loginRes = res;
            if (IsLoginSuccess)
            {
                RefreshSubscribers((int)UpdateType.LoginRes);
            }
            else
            {
                GUIManager.Instance.OpenTipsView(loginRes.Result);
            }
        }

        CSUserRegisterRes registerRes;
        CSUserRegisterRes emptyRegisterRes = new CSUserRegisterRes();
        public CSUserRegisterRes RegisterRes { get { return registerRes ?? emptyRegisterRes; } }
        public void SetRegisterRes(CSUserRegisterRes res)
        {
            Logger.Log("SetRegisterRes " + res.Result);
            registerRes = res;
            RefreshSubscribers((int)UpdateType.RegisterRes);
        }
    }
}
