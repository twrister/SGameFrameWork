using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;

namespace SthGame
{
    public class WorldSessionController : UIChildController
    {
        WorldSessionView view;
        ChatDataStore chatDataStore;

        List<ChatMemberItemController> memberItemContollerList;
        List<ChatMessageItemController> messageItemControllerList;

        protected override string GetResourcePath()
        {
            return "Prefabs/WorldSessionView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as WorldSessionView;
            chatDataStore = DataStoreManager.Instance.FindOrBindDataStore<ChatDataStore>();

            memberItemContollerList = new List<ChatMemberItemController>();
            messageItemControllerList = new List<ChatMessageItemController>();
            UpdateMemberList();
        }

        public void UpdateMemberList()
        {
            var worldSessionData = chatDataStore.GetSessionData(1);
            if (worldSessionData == null) return;

            while (memberItemContollerList.Count < worldSessionData.PlayerList.Count)
            {
                var item = CreateChildController<ChatMemberItemController>(memberItemContollerList.Count, view.chatMemberGrid.gameObject);
                memberItemContollerList.Add(item);
            }

            for (int i = 0; i < memberItemContollerList.Count; i++)
            {
                memberItemContollerList[i].SetActive(i < worldSessionData.PlayerList.Count);
                if (i < worldSessionData.PlayerList.Count)
                {
                    memberItemContollerList[i].SetData(worldSessionData.PlayerList[i]);
                }
            }

            view.onlineText.text = string.Format("在线：{0}", worldSessionData.PlayerList.Count);
        }

        public void UpdateMessageList()
        {
            bool needRepos = false;
            if (view.messageScrollRect.verticalNormalizedPosition < 0.01f)
            {
                needRepos = true;
            }

            var worldSessionData = chatDataStore.GetSessionData(1);
            if (worldSessionData == null) return;
            
            while (messageItemControllerList.Count < worldSessionData.ChatMsgList.Count)
            {
                var item = CreateChildController<ChatMessageItemController>(messageItemControllerList.Count, view.chatMessageGroup.gameObject);
                messageItemControllerList.Add(item);
            }

            for (int i = 0; i < messageItemControllerList.Count; i++)
            {
                messageItemControllerList[i].SetActive(i < worldSessionData.ChatMsgList.Count);
                if (i < worldSessionData.ChatMsgList.Count)
                {
                    messageItemControllerList[i].SetData(worldSessionData.ChatMsgList[i]);
                }
            }

            if (needRepos)
            {
                GameRoot.Instance.StartCoroutine(OoRePos());
            }
        }

        private IEnumerator OoRePos()
        {
            yield return null;
            view.messageScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}