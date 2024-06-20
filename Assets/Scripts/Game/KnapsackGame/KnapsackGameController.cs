using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;
using UnityEngine.UI;

namespace SthGame
{
    public class KnapsackGameController : UIBaseController
    {
        #region 内部参数
        
        private int MapWidth => m_View.m_MapWidth;
        private int MapHeight => m_View.m_MapHeight;
        private int ItemSize => m_View.m_ItemSize;
        
        private KnapsackGameView m_View;
        private int[][] m_Grids;
        bool m_IsPressing;
        private int[][] m_CurData;
        Vector2 m_FirstPressPos;
        bool m_IsFirstPress = true;
        bool m_CanDoMove;

        #endregion

        #region 基类方法

        protected override string GetResourcePath()
        {
            return "Prefabs/KnapsackGameView";
        }

        public override void Init()
        {
            m_View = UINode as KnapsackGameView;
            
            m_View.m_CloseButton.onClick.AddListener(Close);

            GlobalEventSystem.Instance.Bind(EventId.OnKnapsackPointerDown, OnKnapsackPointerDown);
            GlobalEventSystem.Instance.Bind(EventId.OnKnapsackPointerUp, OnKnapsackPointerUp);
        }
        
        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            InitGrids();
        }

        #endregion

        // Update is called once per frame
        void Update()
        {
            if (m_IsPressing)
            {
                Vector2 mousePos = Input.mousePosition;
                if (m_IsFirstPress)
                {
                    m_FirstPressPos = mousePos;
                    m_IsFirstPress = false;
                }
                
                Vector3 offset = mousePos - m_FirstPressPos;
                // Vector3 offset = (mousePos - m_FirstPressPos) * UITools.ScreenScale / m_MahjongScale;

                if (m_CanDoMove)
                {
                    
                }
                else if (offset.magnitude > 20) // 拖拽超过阈值，开始移动
                {
                    m_CanDoMove = true;
                }
            }
        }

        private void OnKnapsackPointerDown(object[] ps)
        {
            m_CurData = ps as int[][];
            m_IsPressing = true;
        }
            
        private void OnKnapsackPointerUp(object[] ps)
        {
            m_IsPressing = false;
        }

        #region 格子显示
        
        List<Image> m_GridList = new List<Image>();

        private void InitGrids()
        {
            if (MapWidth < 1 || MapHeight < 1) return;

            m_GridList.Clear();
            m_View.m_GridPrefab.SetActive(true);
            for (int w = 0; w < MapWidth; w++)
            {
                for (int h = 0; h < MapHeight; h++)
                {
                    GameObject gridObj = GameObject.Instantiate(m_View.m_GridPrefab, m_View.m_GridRoot);
                    Image gridImg = gridObj.GetComponent<Image>();
                    // gridImg.Init();
                    m_GridList.Add(gridImg);
                }
            }
            m_View.m_GridPrefab.SetActive(false);
            
            // Grid位置
            for (int w = 0; w < MapWidth; w++)
            {
                for (int h = 0; h < MapHeight; h++)
                {
                    int index = w * MapHeight + h;
                    PathFindingBaseController.SetLocalPosByGridPos(m_GridList[index].transform, w, h, ItemSize, MapWidth, MapHeight);
                }
            }
        }

        #endregion
    }
}
