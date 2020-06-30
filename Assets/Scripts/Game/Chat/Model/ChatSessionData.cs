using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

namespace SthGame
{
    public class ChatSessionData
    {
        private int ItemMsgCount = 2;
        public int SessionId { get; private set; }
        public int MemberCount { get { return PlayerList == null ? 0 : PlayerList.Count; } }
        public List<PlayerInfo> PlayerList { get; private set; }
        
        public List<ChatMessageData> ChatMsgList { get; set; }

        public ChatSessionData(int sessionId, List<PlayerInfo> playerList)
        {
            SessionId = sessionId;
            PlayerList = playerList;
            ChatMsgList = new List<ChatMessageData>();
        }

        public void OnPlayerListChanged(List<PlayerInfo> playerList)
        {
            PlayerList = playerList;
        }

        public void AddMessage(ChatMessageInfo info)
        {
            ChatMessageData lastMsg = ChatMsgList.Count > 0 ? ChatMsgList[ChatMsgList.Count - 1] : null;

            if (lastMsg == null)
            {
                ChatMessageData msg = new ChatMessageData(info);
                ChatMsgList.Add(msg);
            }
            else
            {
                if (lastMsg.PlayerId == info.SenderInfo.PlayerId 
                    && info.TimeStamp - lastMsg.TimeStamp <= 60
                    && lastMsg.MsgCount < ItemMsgCount)
                {
                    lastMsg.AddMessage(info);
                }
                else
                {
                    ChatMessageData msg = new ChatMessageData(info);
                    ChatMsgList.Add(msg);
                }
            }
            //ChatMsgList
        }
    }
}
