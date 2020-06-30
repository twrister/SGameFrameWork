using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

namespace SthGame
{
    public class ChatDataStore : DataStore
    {
        Dictionary<int, ChatSessionData> sessionDataDict;

        public enum UpdateType
        {
            SessionChanged,
            NewMessage,
        }

        public ChatDataStore()
            : base(DataStoreType.Chat)
        {
            sessionDataDict = new Dictionary<int, ChatSessionData>();
        }

        public void OnCSChatSessionChangedNtf(CSChatSessionChangedNtf ntf)
        {
            //Logger.Log("OnCSChatSessionChangedNtf {0}", ntf.PlayerList.Count);
            if (ntf == null || ntf.SessionId == 0) return;

            if (sessionDataDict.ContainsKey(ntf.SessionId))
            {
                sessionDataDict[ntf.SessionId].OnPlayerListChanged(ntf.PlayerList);
            }
            else
            {
                sessionDataDict.Add(ntf.SessionId, new ChatSessionData(ntf.SessionId, ntf.PlayerList));
            }

            RefreshSubscribers((int)UpdateType.SessionChanged);
        }

        public void OnCSGetSessionInfoRes(CSGetSessionInfoRes res)
        {
            if (res == null || res.SessionId == 0) return;

            if (sessionDataDict.ContainsKey(res.SessionId))
            {
                sessionDataDict[res.SessionId].OnPlayerListChanged(res.PlayerList);
            }
            else
            {
                sessionDataDict.Add(res.SessionId, new ChatSessionData(res.SessionId, res.PlayerList));
            }
            RefreshSubscribers((int)UpdateType.SessionChanged);
        }

        public void OnCSNewChatMessageNtf(CSNewChatMessageNtf ntf)
        {
            if (ntf == null) return;
            for (int i = 0; i < ntf.MessageList.Count; i++)
            {
                ChatMessageInfo info = ntf.MessageList[i];
                if (sessionDataDict.ContainsKey(info.SessionId))
                {
                    sessionDataDict[info.SessionId].AddMessage(info);
                }
            }
            RefreshSubscribers((int)UpdateType.NewMessage);
            FlashWinTool.FlashWindow(FlashWinTool.GetProcessWnd());
        }

        public ChatSessionData GetSessionData(int sessionId)
        {
            if (sessionDataDict.ContainsKey(sessionId))
            {
                return sessionDataDict[sessionId];
            }
            return null;
        }
    }
}
