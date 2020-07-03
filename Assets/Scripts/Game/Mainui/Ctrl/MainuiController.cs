using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using LitJson;

namespace SthGame
{
    public class MainuiController : UIBaseController, DataStoreSubscriber
    {
        MainuiView view;
        UserInfoDataStore userInfoDataStore;

        protected override string GetResourcePath()
        {
            return "Prefabs/MainUIView";
        }

        public override void Init()
        {
            base.Init();
            view = UINode as MainuiView;

            userInfoDataStore = DataStoreManager.Instance.FindOrBindDataStore<UserInfoDataStore>();
            userInfoDataStore.RegisterSubscriber(this);

            view.modeBtn.onClick.AddListener(OnClickModeButton);
            view.headFrameBtn.onClick.AddListener(OnClickHeadFrame);
        }

        protected override void OpenCallBack()
        {
            FlushView();

            GlobalEventSystem.Instance.Fire(EventId.onMainuiOpenCallback);
        }

        private void FlushView()
        {
            PlayerInfo playerInfo = userInfoDataStore.LocalPlayerInfo;
            if (playerInfo == null)
            {
                view.nameText.text = "临时用户";
                view.headImage.LoadSprite("HeadAtlas", string.Format("head_{0}", 0));
                view.coinText.text = "0";
                return;
            }

            view.nameText.text = playerInfo.NickName;
            view.headImage.LoadSprite("HeadAtlas", string.Format("head_{0}", playerInfo.HeadFrameId));
            view.coinText.text = playerInfo.Coin.ToString();
        }

        private void OnClickModeButton()
        {
            //GUIManager.Instance.Open<ExampleListShowController>();

            GUIManager.Instance.Open<FunctionListController>();
        }

        private void OnClickHeadFrame()
        {
            PlayerInfo playerInfo = userInfoDataStore.LocalPlayerInfo;
            if (playerInfo == null) return;

            GUIManager.Instance.OpenHeadFrameChoose(playerInfo.HeadFrameId, choosedDel:
                (idx) => {
                    GUIManager.Instance.OpenTipsView("更换头像需要花费10块，确认更换？", "确定", 
                        () => {
                            SendChangeHeadImgReq(idx);
                        }, "取消");
                });
        }

        private void SendChangeHeadImgReq(int headId)
        {
            //Logger.Log("SendChangeHeadImgReq {0}", headId);
            CSChangedHeadImageReq req = new CSChangedHeadImageReq();
            req.NewHeadIndex = headId;
            NetworkSystem.Instance.SendEvent(req);
        }

        public void NotifyDataStoreUpdated(DataStore sourceDataStore, int index)
        {
            if (index == (int)UserInfoDataStore.UpdateType.PlayerInfoChanged)
            {
                FlushView();
            }
        }
    }
}