using System;
using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;
using UnityEngine.UI;

public struct KnapsackGridData
{
    public int m_Width;
    public int m_Height;
    public int[] m_Datas;
    public RectTransform m_RectTrans;
}   

public class KnapsackItem : MonoBehaviour
{
    private int m_ItemId;

    public float m_GridSize = 50f;
    public GameObject m_GridPrefab;
    public Color m_Color;
    public List<string> m_Config;

    private KnapsackGridData m_ItemData;
    private List<Image> m_ItemList;

    public void Start()
    {
        Init();
    }

    public void OnPointerDown()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnKnapsackPointerDown, m_ItemData, transform as RectTransform);
    }

    public void OnPointerUp()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnKnapsackPointerUp, m_ItemData, transform as RectTransform);
    }

    public void Init()
    {
        if (m_Config == null || m_Config.Count == 0)
        {
            return;
        }
        
        m_ItemData.m_Width = m_Config[0].Length;
        m_ItemData.m_Height = m_Config.Count;
        m_ItemData.m_Datas = new int[m_ItemData.m_Width * m_ItemData.m_Height];
        m_ItemData.m_RectTrans = transform as RectTransform;
        
        m_GridPrefab.SetActive(true);
        for (int h = 0; h < m_Config.Count; h++)
        {
            for (int w = 0; w < m_Config[h].Length; w++)
            {
                char c = m_Config[h][w];
                if (c == '1')
                {
                    GameObject gridObj = Instantiate(m_GridPrefab, transform);
                    Image img = gridObj.GetComponent<Image>();
                    img.color = m_Color;
                    PathFindingBaseController.SetLocalPosByGridPos(img.transform, w, h, m_GridSize, m_ItemData.m_Width, m_ItemData.m_Height);
                    m_ItemData.m_Datas[h * m_ItemData.m_Width + w] = 1;
                }
            }
        }
        m_GridPrefab.SetActive(false);

        m_ItemData.m_RectTrans.sizeDelta = new Vector2(m_ItemData.m_Width * m_GridSize, m_ItemData.m_Height * m_GridSize);
    }
}
