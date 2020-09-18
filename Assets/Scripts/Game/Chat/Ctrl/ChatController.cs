using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;
using EmojiText.Taurus;

namespace SthGame
{
    public class ChatController : UIBaseController, DataStoreSubscriber
    {
        ChatView view;
        bool viewIsOn;

        ChatDataStore chatDataStore;
        WorldSessionController worldSessionCtrl;
        int curSessionId = 1;
        bool showEmojiPanel = false;

        CSSendChatMessageReq tmpMsgReq;

        protected override string GetResourcePath()
        {
            return "Prefabs/ChatView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as ChatView;
            chatDataStore = DataStoreManager.Instance.FindOrBindDataStore<ChatDataStore>();
            chatDataStore.RegisterSubscriber(this);

            tmpMsgReq = new CSSendChatMessageReq();
            view.InitViewPos();
            InitView();

            view.switchBtn.onClick.AddListener(OnClickSwitch);
            view.sendBtn.onClick.AddListener(OnClickSend);
            view.emojiBtn.onClick.AddListener(OnClickEmoji);
            view.emojiBgBtn.onClick.AddListener(OnClickEmojiBg);

            GlobalEventSystem.Instance.Bind(EventId.onClickChatEmojiItem, OnClickEmojiItem);
            GlobalEventSystem.Instance.Bind(EventId.onClickKeyboardEnter, OnClickKeyboardEnter);
        }

        public override void ShutDown()
        {
            base.ShutDown();
            chatDataStore.UnRegisterSubscriber(this);

            GlobalEventSystem.Instance.UnBind(EventId.onClickChatEmojiItem, OnClickEmojiItem);
            GlobalEventSystem.Instance.UnBind(EventId.onClickKeyboardEnter, OnClickKeyboardEnter);
        }

        private void InitView()
        {
            worldSessionCtrl = CreateChildController<WorldSessionController>(view.chatSessionsRoot, Vector3.zero);

            InitEmojiPanel();
        }

        private void OnClickSwitch()
        {
            viewIsOn = !viewIsOn;
            view.OnSwitchView(viewIsOn);

            if (viewIsOn)
            {
                SendSessionInfoReq();
                view.redObj.SetActive(false);
            }
        }

        private void SendSessionInfoReq()
        {
            CSGetSessionInfoReq req = new CSGetSessionInfoReq();
            req.SessionId = 1;
            NetworkSystem.Instance.SendEvent(req);
        }

        public void NotifyDataStoreUpdated(DataStore sourceDataStore, int index)
        {
            if (index == (int)ChatDataStore.UpdateType.SessionChanged)
            {
                worldSessionCtrl.UpdateMemberList();
            }
            else if (index == (int)ChatDataStore.UpdateType.NewMessage)
            {
                worldSessionCtrl.UpdateMessageList();
                if (!viewIsOn)
                {
                    view.redObj.SetActive(true);
                }
            }
        }

        protected override void OpenCallBack()
        {

        }

        private void OnClickSend()
        {
            if (string.IsNullOrEmpty(view.chatInputField.text)) return;

            tmpMsgReq.SessionId = curSessionId;
            tmpMsgReq.Content = view.chatInputField.text;
            NetworkSystem.Instance.SendEvent(tmpMsgReq);

            view.chatInputField.text = "";
            view.chatInputField.ActivateInputField();
        }

        private void OnClickEmoji()
        {
            showEmojiPanel = !showEmojiPanel;
            view.emojiPanel.SetActive(showEmojiPanel);
        }

        private void OnClickEmojiBg()
        {
            showEmojiPanel = false;
            view.emojiPanel.SetActive(showEmojiPanel);
        }

        private void InitEmojiPanel()
        {
            view.InitEmojiPanel();
            view.emojiPanel.SetActive(showEmojiPanel);
        }

        private void OnClickEmojiItem(object[] ps)
        {
            if (ps.Length > 0)
            {
                int emojiIdx = (int)ps[0];
                view.chatInputField.text += string.Format("[#{0:00}]", emojiIdx);
            }
        }

        public InlineManager GetInlineMng()
        {
            return view.GetInlineMng();
        }

        private void OnClickKeyboardEnter(object[] ps)
        {
            if (viewIsOn)
            {
                OnClickSend();
            }
        }
    }
}