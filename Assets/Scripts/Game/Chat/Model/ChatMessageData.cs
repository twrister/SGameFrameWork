using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

namespace SthGame
{
    public class ChatMessageData
    {
        public int SessionId { get; private set; }
        private PlayerInfo playerInfo;

        private ChatMessageInfo msgInfo;
        public int PlayerId { get { return playerInfo != null ? playerInfo.PlayerId : 0; } }
        public string SenderName { get { return playerInfo != null ? playerInfo.NickName : ""; } }
        public int HeadFrameId { get { return playerInfo != null ? playerInfo.HeadFrameId : 0; } }
        public uint TimeStamp { get; private set; }
        public int MsgCount { get; set; }
        public string Message { get; set; }

        public ChatMessageData(ChatMessageInfo info)
        {
            SessionId = info.SessionId;
            playerInfo = info.SenderInfo;
            TimeStamp = info.TimeStamp;
            AddMessage(info);
        }

        public void AddMessage(ChatMessageInfo info)
        {
            Message += string.Format("{0}\n", info.Content);
            MsgCount++;
        }
    }
}
