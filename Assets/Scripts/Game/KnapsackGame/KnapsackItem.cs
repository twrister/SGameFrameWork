using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;

public class KnapsackItem : MonoBehaviour
{
    private int m_ItemId;

    public int[][] m_ItemData;
    
    
    public void OnPointerDown()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerDown, m_ItemData);
    }

    public void OnPointerUp()
    {
        GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerUp, m_ItemData);
    }
}
