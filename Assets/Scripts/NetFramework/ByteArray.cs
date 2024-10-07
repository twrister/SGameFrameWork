using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray
{
    /// <summary>
    /// 默认长度
    /// </summary>
    private const int DEFAULT_SIZE = 1024;

    /// <summary>
    /// 字节数组
    /// </summary>
    public byte[] m_Bytes;

    /// <summary>
    /// 读的位置
    /// </summary>
    public int m_ReadIndex;

    /// <summary>
    /// 写的位置
    /// </summary>
    public int m_WriteIndex;

    /// <summary>
    /// 初始容量
    /// </summary>
    public int m_InitSize;
    
    /// <summary>
    /// 数组容量
    /// </summary>
    public int m_Capacity;
    
    /// <summary>
    /// 读写间的长度
    /// </summary>
    public int Length => m_WriteIndex - m_ReadIndex;

    /// <summary>
    /// 余量
    /// </summary>
    public int Remain => m_Capacity - m_WriteIndex;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="size"></param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        m_Bytes = new byte[size];
        m_InitSize = size;
        m_Capacity = size;
        m_ReadIndex = 0;
        m_WriteIndex = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bytes"></param>
    public ByteArray(byte[] bytes)
    {
        m_Bytes = bytes;
        m_Capacity = bytes.Length;
        m_ReadIndex = 0;
        m_WriteIndex = 0;
    }

    /// <summary>
    /// 移动数据
    /// </summary>
    public void MoveBytes()
    {
        if (Length > 0)
        {
             Array.Copy(m_Bytes, m_ReadIndex, m_Bytes, 0, Length);
        }
        m_WriteIndex = Length;
        m_ReadIndex = 0;
    }

    public void Resize(int size)
    {
        if (size < Length || size < m_InitSize)
        {
            return;
        }

        m_Capacity = size;
        
        // 新数组
        byte[] newBytes = new byte[m_Capacity];
        Array.Copy(m_Bytes, m_ReadIndex, newBytes, 0, Length);
        m_Bytes = newBytes;
        m_WriteIndex = Length;
        m_ReadIndex = 0;
    }
}
