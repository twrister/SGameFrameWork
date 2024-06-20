using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;
using UnityEngine.UI;

public class KnapsackGameController : UIBaseController
{
    public Transform m_GridRoot;
    public GameObject m_GridPrefab;
    public int m_MapWidth = 6;
    public int m_MapHeight = 4;
    // public List<MahjongChessItem> m_ItemList;
    private int[][] m_Grids;
    
    bool m_IsPressing;
    private int[][] m_CurData;
    Vector2 m_FirstPressPos;
    bool m_IsFirstPress = true;
    bool m_CanDoMove;

    #region 基类方法

    protected override string GetResourcePath()
    {
        return "Prefabs/BFSPathFindingView";
    }

    public override void Init()
    {
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
        if (m_MapWidth < 1 || m_MapHeight < 1) return;

        m_GridList.Clear();
        for (int w = 0; w < m_MapWidth; w++)
        {
            for (int h = 0; h < m_MapHeight; h++)
            {
                GameObject gridObj = GameObject.Instantiate(m_GridPrefab, m_GridRoot);
                Image gridImg = gridObj.GetComponent<Image>();
                // gridImg.Init();
                m_GridList.Add(gridImg);
            }
        }
    }

    #endregion
}
