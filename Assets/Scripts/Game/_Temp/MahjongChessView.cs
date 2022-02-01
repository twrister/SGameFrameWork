using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SthGame
{
    public class MahjongChessView : UIBaseView
    {
        public MahjongChessItem m_ItemPrefab;
        public Transform m_ItemsRoot;
        public Button m_CloseBtn;
        public Button m_RandomBtn;
        public Button m_ResetBtn;
        public Button m_RoolbackBtn;
        public Button m_TipsBtn;
        public Button m_ViolenceTestBtn;

        UnityAction m_LateUpdate;

        private void LateUpdate()
        {
            if (m_LateUpdate != null) m_LateUpdate();
        }

        public void RegistLateUpdate(UnityAction lateUpdate)
        {
            m_LateUpdate = lateUpdate;
        }
    }
}