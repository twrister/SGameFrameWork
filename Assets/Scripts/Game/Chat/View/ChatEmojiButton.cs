using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class ChatEmojiButton : MonoBehaviour
    {
        public Image emojiImage;
        public Button emojiBtn;

        int index = -1;
        bool isInitView = false;

        private void OnEnable()
        {
            if (isInitView) return;
            isInitView = true;
            emojiBtn.onClick.AddListener(OnClickEmoji);
        }

        public void SetData(int idx)
        {
            index = idx;
            emojiImage.LoadSprite("EmojiAtlas", string.Format("EmojiOne_{0}", index.ToString()));
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private void OnClickEmoji()
        {
            if (index < 0) return;

            GlobalEventSystem.Instance.Fire(EventId.onClickChatEmojiItem, index + 1);
        }
    }
}
