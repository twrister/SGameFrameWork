using System;
using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;

public class KnapsackItem : MonoBehaviour
{
    private int m_ItemId;

    public List<string> m_Config;

    public int m_Width;
    public int m_Height;
    public int[] m_ItemData;
    
    public void Start()
    {
        Init();
    }

    public void OnPointerDown()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerDown, m_ItemData);
    }

    public void OnPointerUp()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerUp, m_ItemData);
    }

    public void Init()
    {
        int width = 0;
        // m_Height = 
        for (int i = 0; i < m_Config.Count; i++)
        {
            
        }
        
        // m_ItemData = new int[]
    }
}
