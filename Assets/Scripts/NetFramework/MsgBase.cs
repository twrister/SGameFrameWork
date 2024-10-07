using System;
using System.Text;
using UnityEngine;

namespace Tools
{
    public class MsgBase
    {
        public string m_ProtoName = "";
        
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] Encode(MsgBase msgBase)
        {
            string s = JsonUtility.ToJson(msgBase);
            return Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="protoName"></param>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            string s = Encoding.UTF8.GetString(bytes, offset, count);
            return (MsgBase)JsonUtility.FromJson(s, Type.GetType(protoName));
        }

        /// <summary>
        /// 协议名编码
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] EncodeName(MsgBase msgBase)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(msgBase.m_ProtoName);
            short len = (short)nameBytes.Length;
            byte[] bytes = new byte[len + 2];
            
            // bytes[0] = (byte)(len % 256);
            // bytes[1] = (byte)(len / 256);
            bytes[0] = (byte)(len & 0x00ff);
            bytes[1] = (byte)(len >> 8);
            
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// 协议名解码
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            if (offset + 2 > bytes.Length)
            {
                return "";
            }
            
            // short len = (short)(bytes[offset + 1] * 256 + bytes[offset]);
            short len = (short)(bytes[offset + 1] << 8 | bytes[offset]);
            if (len <= 0)
            {
                return "";
            }
            
            count = 2 + len;
            return Encoding.UTF8.GetString(bytes, offset + 2, len);
        }
    }
}