using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;
using EmojiText.Taurus;

namespace SthGame
{
    public class ChatSystem : ClientSystem
    {
        ChatDataStore chatDataStore;
        public static ChatSystem Instance { get; private set; }
        ChatController chatController;
        public InlineManager InlineMng { get; private set; }

        public override void Init()
        {
            Instance = this;
            chatDataStore = DataStoreManager.Instance.FindOrBindDataStore<ChatDataStore>();

            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSChatSessionChangedNtf, OnCSChatSessionChangedNtf);
            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSGetSessionInfoRes, OnCSGetSessionInfoRes);
            NetworkSystem.Instance.RegisterNetworkEvent(Common.CSNewChatMessageNtf, OnCSNewChatMessageNtf);

            GlobalEventSystem.Instance.Bind(EventId.onMainuiOpenCallback, OnMainuiOpenCallBack);
        }

        public override void ShutDown()
        {
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSChatSessionChangedNtf);
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSGetSessionInfoRes);
            NetworkSystem.Instance.UnRegisterNetworkEvent(Common.CSNewChatMessageNtf);

            GlobalEventSystem.Instance.UnBind(EventId.onMainuiOpenCallback, OnMainuiOpenCallBack);
        }

        private void OnCSChatSessionChangedNtf(string jsonStr)
        {
            CSChatSessionChangedNtf ntf = JsonMapper.ToObject<CSChatSessionChangedNtf>(jsonStr);
            chatDataStore.OnCSChatSessionChangedNtf(ntf);
        }

        private void OnCSGetSessionInfoRes(string jsonStr)
        {
            CSGetSessionInfoRes res = JsonMapper.ToObject<CSGetSessionInfoRes>(jsonStr);
            chatDataStore.OnCSGetSessionInfoRes(res);
        }

        private void OnCSNewChatMessageNtf(string jsonStr)
        {
            CSNewChatMessageNtf ntf = JsonMapper.ToObject<CSNewChatMessageNtf>(jsonStr);
            chatDataStore.OnCSNewChatMessageNtf(ntf);
        }

        private void OnMainuiOpenCallBack(object[] ps)
        {
            //chatController = GUIManager.Instance.Open<ChatController>();
            //InlineMng = chatController.GetInlineMng();
        }
    }
}