using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace SthGame
{
    public class ChatMemberItemController : UIChildController
    {
        ChatMemberItemView view;
        int headFrameIdx = -1;

        protected override string GetResourcePath()
        {
            return "Prefabs/ChatMemberItem";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as ChatMemberItemView;
        }

        public void SetData(PlayerInfo playerInfo)
        {
            view.nickNameText.text = playerInfo.NickName;
            view.headFrameImage.LoadSprite("HeadAtlas", string.Format("head_{0}", playerInfo.HeadFrameId));
        }
    }
}