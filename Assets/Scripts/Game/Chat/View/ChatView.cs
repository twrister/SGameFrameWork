using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using EmojiText.Taurus;

namespace SthGame
{
    public class ChatView : UIBaseView
    {
        public GameObject rootObj;

        public Button switchBtn;
        public InputField chatInputField;
        public Button sendBtn;
        public GameObject chatSessionsRoot;
        public GameObject onObj;
        public GameObject offObj;
        public Button emojiBtn;
        public GameObject emojiPanel;
        public GameObject emojiRoot;
        public ChatEmojiButton emojiItem;
        public Button emojiBgBtn;
        public GameObject redObj;

        bool viewIsOn = false;

        public void OnSwitchView(bool isOn)
        {
            viewIsOn = isOn;
            transform.DOLocalMoveX(isOn ? 420f : 0f, 0.3f);
            UpdateView();
        }

        public void InitViewPos()
        {
            transform.localPosition = Vector3.zero;
            redObj.SetActive(false);
            UpdateView();
        }

        private void UpdateView()
        {
            onObj.SetActive(!viewIsOn);
            offObj.SetActive(viewIsOn);
        }

        public void AddEmojiItem(int index)
        {
            GameObject item = Instantiate(emojiItem.gameObject);
            item.transform.SetParent(emojiRoot.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
            ChatEmojiButton emojiBtn = item.GetComponent<ChatEmojiButton>();
            emojiBtn.SetData(index);
        }

        public void InitEmojiPanel()
        {
            emojiItem.SetActive(true);
            for (int i = 0; i < 16; i++)
            {
                AddEmojiItem(i);
            }
            emojiItem.SetActive(false);
        }

        public InlineManager GetInlineMng()
        {
            return transform.GetComponent<InlineManager>();
        }
    }

}