using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace SthGame
{
    public class HeadFrameItemController : UIChildController
    {
        HeadFrameItemView view;
        int headFrameIdx = -1;

        protected override string GetResourcePath()
        {
            return "Prefabs/HeadFrameItem";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as HeadFrameItemView;

            view.headFrameBtn.onClick.AddListener(OnClickHeadFrame);
        }

        public void SetData(int index, int chooseIdx)
        {
            if (headFrameIdx != index)
            {
                headFrameIdx = index;
                view.headFrameImage.LoadSprite("HeadAtlas", string.Format("head_{0}", index));
                headFrameIdx = index;
            }
            view.chooseObj.SetActive(index == chooseIdx);
        }

        public void OnClickHeadFrame()
        {
            GlobalEventSystem.Instance.Fire(EventId.onClickHeadFrameChooseItem, headFrameIdx);
        }
    }
}