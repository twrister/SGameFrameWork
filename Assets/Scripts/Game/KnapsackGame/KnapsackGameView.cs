using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class KnapsackGameView : UIBaseView
    {
        public Transform m_GridRoot;
        public GameObject m_GridPrefab;
        public int m_MapWidth = 6;
        public int m_MapHeight = 4;
        public int m_ItemSize = 50;
        
        public Button m_CloseButton;
    }
}