using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace SthGame
{
    public class ChatMessageItemController : UIChildController
    {
        ChatMessageItemView view;

        protected override string GetResourcePath()
        {
            return "Prefabs/ChatMessageItem";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as ChatMessageItemView;
        }

        public void SetData(ChatMessageData message)
        {
            view.nickNameText.text = message.SenderName;
            view.headFrameImage.LoadSprite("HeadAtlas", string.Format("head_{0}", message.HeadFrameId));
            view.inlineText.text = message.Message;
            view.timeText.text = GetTimeStr(message.TimeStamp);
        }

        private string GetTimeStr(long timeStamp)
        {

            uint nowTimeStamp = DateTimeTools.GetCurrentTimeStamp();
            long leftTime = nowTimeStamp - timeStamp < 0 ? 0 : nowTimeStamp - timeStamp;

            string str = "";
            if (leftTime < 60)
            {
                str = "刚刚";
            }
            else if (leftTime < 3600)
            {
                str = string.Format("{0}分钟前", (leftTime / 60).ToString());
            }
            else
            {
                str = string.Format("{0}小时前", (leftTime / 3600).ToString());
            }

            return str;
        }
    }
}