using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Protocol;
using LitJson;

namespace SthGame
{
    public class NetworkSystem : ClientSystem, IPhotonPeerListener
    {
        public static NetworkSystem Instance { get; private set; }

        public static PhotonPeer Peer { get { return peer; } }
        private static PhotonPeer peer;

        private NetworkEventChannel networkEventChannel;
        public override void Init()
        {
            Instance = this;

            networkEventChannel = new NetworkEventChannel();
            networkEventChannel.Init();
        }

        public void InitClientPeer()
        {
            peer = new PhotonPeer(this, ConnectionProtocol.Udp);
            peer.Connect("10.32.193.39:5055", "SthGame1");
        }

        public override void ShutDown()
        {
            if (peer != null && peer.PeerState == PeerStateValue.Connected)
            {
                peer.Disconnect();
            }
        }

        public override void Tick(float deltaTime)
        {
            if (peer != null) peer.Service();
        }

        #region IPhotonPeerListener
        public void DebugReturn(DebugLevel level, string message)
        {
            Logger.Log(string.Format("DebugReturn- level: {0} message: {1}", level, message));
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            Logger.Log(statusCode.ToString());
            if (statusCode == StatusCode.Connect)
            {
                GlobalEventSystem.Instance.Fire(EventId.onConnectNetwork);
            }
            else if (statusCode == StatusCode.Disconnect)
            {
                GUIManager.Instance.OpenTipsView("与服务器断开连接", "重新连接", () => {
                    GameRoot.Instance.DoReLogin();
                });
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case 1:
                    ProcessParameters(operationResponse.Parameters);
                    break;
                default:
                    break;
            }
        }

        public void OnEvent(EventData eventData)
        {
            switch (eventData.Code)
            {
                case 1:
                    ProcessParameters(eventData.Parameters);
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void ProcessParameters(Dictionary<byte, object> data)
        {
            object codeObj;
            object response;
            data.TryGetValue(1, out codeObj);
            data.TryGetValue(2, out response);
            Common code = (Common)codeObj;
            string jsonStr = response as string;

            Logger.Log("ProcessParameters, code = {0}, jsonStr = {1}", code, jsonStr);
            networkEventChannel.OnNetworkEvent(code, jsonStr);
        }

        EventData eventData = new EventData();
        public void SendEvent(Message req)
        {
            string json = JsonMapper.ToJson(req);
            Dictionary<byte, object> datas = new Dictionary<byte, object>();
            datas.Add(1, req.Code);
            datas.Add(2, json);
            if (peer == null)
            {
                GUIManager.Instance.OpenTipsView("链接服务器失败", "确定", () => { });
            }
            else
            {
                peer.OpCustom(1, datas, true);
            }
        }

        public void RegisterNetworkEvent(Common code, NetworkEventChannel.NetworkDelegate handler)
        {
            networkEventChannel.RegisterNetworkEvent(code, handler);
        }

        public void UnRegisterNetworkEvent(Common code)
        {
            networkEventChannel.UnRegisterNetworkEvent(code);
        }
    }
}
