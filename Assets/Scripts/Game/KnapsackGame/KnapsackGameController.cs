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
        private float ItemSize => m_View.m_ItemSize;
        
        private KnapsackGameView m_View;
        private int[][] m_Grids;
        bool m_IsPressing;
        Vector2 m_FirstPressPos;
        bool m_IsFirstPress = true;
        bool m_CanDoMove;
        private KnapsackGridData m_BgGridData;

        private Vector2 m_FirstBgGridPos;
        private KnapsackGridData m_CurSelectData;
        // private RectTransform m_CurItem;
        private Vector2 m_FirstPos;
        
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
            m_View.RegistUpdateCallback(UpdateCallback);
            
            GlobalEventSystem.Instance.Bind(EventId.OnKnapsackPointerDown, OnKnapsackPointerDown);
            GlobalEventSystem.Instance.Bind(EventId.OnKnapsackPointerUp, OnKnapsackPointerUp);
        }
        
        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            InitGrids();
        }

        #endregion

        private void OnKnapsackPointerDown(object[] ps)
        {
            Logger.Log("OnPointerDown");

            m_CurSelectData = (KnapsackGridData)ps[0];
            m_FirstPos = m_CurSelectData.m_RectTrans.anchoredPosition;
            m_IsPressing = true;
        }
            
        private void OnKnapsackPointerUp(object[] ps)
        {
            Logger.Log("OnPointerUp");

            m_CurSelectData.m_RectTrans.anchoredPosition = m_FirstPos;
            m_IsPressing = false;
            m_CanDoMove = false;
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
                    m_GridList.Add(gridImg);
                }
            }
            m_View.m_GridPrefab.SetActive(false);

            m_BgGridData.m_Width = MapWidth;
            m_BgGridData.m_Height = MapHeight;
            m_BgGridData.m_Datas = new int[MapWidth * MapHeight];
            // Grid位置
            for (int w = 0; w < MapWidth; w++)
            {
                for (int h = 0; h < MapHeight; h++)
                {
                    int index = h * MapWidth + w;
                    PathFindingBaseController.SetLocalPosByGridPos(m_GridList[index].transform, w, h, ItemSize, MapWidth, MapHeight);
                    m_BgGridData.m_Datas[index] = 0;
                }
            }
            m_BgGridData.m_RectTrans = m_View.m_GridRoot as RectTransform;

            m_FirstBgGridPos = m_View.m_GridRoot.anchoredPosition + (m_GridList[0].transform as RectTransform).anchoredPosition;
        }

        #endregion

        #region update

        private void UpdateCallback()
        {
            if (m_IsPressing)
            {
                Vector2 mousePos = Input.mousePosition;
                if (m_IsFirstPress)
                {
                    m_FirstPressPos = mousePos;
                    m_IsFirstPress = false;
                }
                
                Vector2 offset = (mousePos - m_FirstPressPos) * UITools.ScreenScale;
                // Vector3 offset = (mousePos - m_FirstPressPos) * UITools.ScreenScale / m_MahjongScale;

                if (m_CanDoMove)
                {
                    if (m_CurSelectData.m_RectTrans != null)
                    {
                        m_CurSelectData.m_RectTrans.anchoredPosition = m_FirstPos + offset;
                    }
                    
                    // 找到前景左下对应背景的位置
                    Vector2 fgOriginPos = m_CurSelectData.m_RectTrans.anchoredPosition + GetOriginGridOffset(m_CurSelectData, ItemSize);
                    Vector2 bgOriginPos = m_BgGridData.m_RectTrans.anchoredPosition + GetOriginGridOffset(m_BgGridData, ItemSize);

                    Vector2 originOffset = fgOriginPos - bgOriginPos;
                    Vector2Int coor = new Vector2Int(Mathf.RoundToInt(originOffset.x / ItemSize), Mathf.RoundToInt(originOffset.y / ItemSize));
                    
                    // 匹配
                    bool match = CheckIsMatch(m_BgGridData, m_CurSelectData, coor, out HashSet<int> overlapGrids);
                    
                    for (int i = 0; i < m_GridList.Count; i++)
                    {
                        if (overlapGrids.Contains(i))
                        {
                            m_GridList[i].color = match ? Color.green : Color.red;
                        }
                        else
                        {
                            m_GridList[i].color = Color.white;
                        }
                    }

                }
                else if (offset.magnitude > 20) // 拖拽超过阈值，开始移动
                {
                    m_CanDoMove = true;
                }
            }
        }

        #endregion

        #region 判断相交逻辑

        private HashSet<int> m_CacheGrids = new HashSet<int>();
        
        private bool CheckIsMatch(KnapsackGridData bg, KnapsackGridData fg, Vector2Int originCoor, out HashSet<int> overlapGrids)
        {
            overlapGrids = m_CacheGrids;
            m_CacheGrids.Clear();
            
            // if (GetGridValue(bg, originCoor) == -1)
            // {
            //     return false;
            // }
            
            bool match = true;
            // int bgIdx = bgXPos * bg.m_Height + bgYPos;
            for (int i = 0; i < fg.m_Datas.Length; i++)
            {
                if (fg.m_Datas[i] == 1)
                {
                    Vector2Int fg_Coor_L = Index2Coor(i, fg.m_Width);
                    Vector2Int bg_Coor = originCoor + fg_Coor_L;

                    int value = GetGridValue(bg, bg_Coor);
                    if (value != 0)
                    {
                        match = false;
                    }

                    int overlapIdx = Coor2Index(bg_Coor, bg);
                    Logger.Log($"bg_Coor = {bg_Coor}, width = { bg.m_Width}, overlapIdx = {overlapIdx}");
                    
                    m_CacheGrids.Add(overlapIdx);
                }
            }
            
            
            return match;
        }

        private int GetGridValue(KnapsackGridData gridData, Vector2Int coor)
        {
            int index = Coor2Index(coor, gridData);
            if (index < 0 || index >= gridData.m_Datas.Length)
            {
                return -1;
            }

            return gridData.m_Datas[index];
        }

        public static Vector2 GetOriginGridOffset(KnapsackGridData gridData, float edge = 50f)
        {
            return new Vector2((1 - gridData.m_Width) * edge * 0.5f, (1 - gridData.m_Height) * edge * 0.5f);
        }

        public static int Coor2Index(Vector2Int coor, KnapsackGridData data)
        {
            if (coor.x < 0 || coor.x >= data.m_Width || coor.y < 0 || coor.y > data.m_Height)
            {
                return -1;
            }
            
            return coor.y * data.m_Width + coor.x;
        }
        
        public static Vector2Int Index2Coor(int index, int width)
        {
            return new Vector2Int(index % width, index / width);
        }

        #endregion
        
    }
}
