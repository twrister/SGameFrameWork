using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SthGame
{
    public class KnapsackGameView : UIBaseView
    {
        public RectTransform m_GridRoot;
        public GameObject m_GridPrefab;
        public int m_MapWidth = 6;
        public int m_MapHeight = 4;
        public float m_ItemSize = 50;
        
        public Button m_CloseButton;
        
        UnityAction m_UpdateCallback;

        private void Update()
        {
            if (m_UpdateCallback != null) m_UpdateCallback();
        }

        public void RegistUpdateCallback(UnityAction updateCallback)
        {
            m_UpdateCallback = updateCallback;
        }
    }
}