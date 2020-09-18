using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace SthGame
{
    public class HeadFrameChooseController : UIBasePopupController
    {
        public delegate void ChoosedDelegate(int idx);
        ChoosedDelegate chooseDelegate;

        HeadFrameChooseView view;
        int curIndex = 0;
        int headFrameCount = 16;
        List<HeadFrameItemController> itemList;

        protected override string GetResourcePath()
        {
            return "Prefabs/HeadFrameChooseView";
        }

        public override void Init()
        {
            base.Init();

            view = UINode as HeadFrameChooseView;

            view.confirmBtn.onClick.AddListener(OnClickConfirm);
            GlobalEventSystem.Instance.Bind(EventId.onClickHeadFrameChooseItem, OnClickHeadFrameChooseItem);

            InitView();
        }

        public override void ShutDown()
        {
            base.ShutDown();

            view.confirmBtn.onClick.RemoveListener(OnClickConfirm);
            GlobalEventSystem.Instance.UnBind(EventId.onClickHeadFrameChooseItem, OnClickHeadFrameChooseItem);
        }


        protected override void OpenCallBack()
        {
            FlushView();
        }

        private void OnClickConfirm()
        {
            Close();
            if (chooseDelegate != null)
            {
                chooseDelegate(curIndex);
            }
        }

        public void SetData(int defaultIdx, ChoosedDelegate chooseDel = null)
        {
            chooseDelegate = chooseDel;
            curIndex = defaultIdx;
        }

        private void InitView()
        {
            itemList = new List<HeadFrameItemController>();
            for (int i = 0; i < headFrameCount; i++)
            {
                var item = CreateChildController<HeadFrameItemController>(view.headFrameGrid.gameObject);
                itemList.Add(item);
            }
        }

        private void FlushView()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].SetData(i, curIndex);
            }
        }

        private void OnClickHeadFrameChooseItem(object[] ps)
        {
            if (ps.Length > 0)
            {
                curIndex = (int)ps[0];
                FlushView();
            }
        }
    }
}