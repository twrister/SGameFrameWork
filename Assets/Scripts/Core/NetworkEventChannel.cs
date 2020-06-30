using System.Collections;
using System.Collections.Generic;
using Protocol;
using LitJson;

namespace SthGame
{
    public class NetworkEventChannel
    {
        public delegate void NetworkDelegate(string jsonStr);
        private Dictionary<Common, NetworkDelegate> networkEvents = new Dictionary<Common, NetworkDelegate>();

        public void RegisterNetworkEvent(Common code, NetworkDelegate handler)
        {
            if (handler == null)
            {
                if (networkEvents.ContainsKey(code))
                {
                    networkEvents.Remove(code);
                }
                return;
            }
            if (!networkEvents.ContainsKey(code))
            {
                networkEvents.Add(code, handler);
            }
            else
            {
                networkEvents[code] = handler;
            }
        }

        public void UnRegisterNetworkEvent(Common code)
        {
            if (networkEvents.ContainsKey(code))
            {
                networkEvents.Remove(code);
            }
        }

        public void OnNetworkEvent(Common code, string jsonStr)
        {
            NetworkDelegate handler;
            if (networkEvents.TryGetValue(code, out handler))
            {
                handler(jsonStr);
            }
        }

        public void Init()
        {
            //RegisterNetworkEvent(Common.CSUserLoginRes, OnCSUserLoginRes);
            RegisterNetworkEvent(Common.CSChangedHeadImageRes, OnCSChangedHeadImageRes);
        }

        private void OnCSChangedHeadImageRes(string jsonStr)
        {
            CSChangedHeadImageRes res = JsonMapper.ToObject<CSChangedHeadImageRes>(jsonStr);
            if (res.Success)
            {
                var ds = DataStoreManager.Instance.FindOrBindDataStore<UserInfoDataStore>();
                ds.LocalPlayerInfo = res.NewPlayerInfo;
            }
            else
            {
                GUIManager.Instance.OpenTipsView(res.Result);
            }
        }

        //private void OnCSUserRegisterRes(string jsonStr)
        //{
        //    CSUserRegisterRes res = JsonMapper.ToObject<CSUserRegisterRes>(jsonStr);
        //}
    }
}