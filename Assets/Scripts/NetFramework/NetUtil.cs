using System;
using System.Net.Sockets;

namespace Tools
{
    public static class NetUtil
    {
        private static Socket m_Socket;

        /// <summary>
        /// 字节数组
        /// </summary>
        private static ByteArray m_ByteArray;

        public static void Init()
        {
            m_Socket ??= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ByteArray ??= new ByteArray();
        }

        public static void Connect(string ip, int port)
        {
            Init();

            m_Socket.BeginConnect(ip, port, ConnectCallback, m_Socket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                Logger.Log("NetUtil.ConnectCallback: 连接成功");
                
                // 接收消息
                socket.BeginReceive(m_ByteArray.m_Bytes, m_ByteArray.m_WriteIndex, m_ByteArray.Remain, 0, ReceiveCallback, socket);
            }
            catch (Exception e)
            {
                Logger.Log($"NetUtil.ConnectCallback: 连接失败 {e.Message}");
            }
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                // 收到的数据量
                int count = socket.EndReceive(ar);
                // 断开连接
                if (count == 0)
                {
                    Close();
                    return;
                }
                // 接收数据
                m_ByteArray.m_WriteIndex += count;
                
                // 处理消息
                OnReceiveData();
                // 长度过小，扩容
                if (m_ByteArray.Remain < 8)
                {
                    m_ByteArray.MoveBytes();
                    m_ByteArray.Resize(m_ByteArray.Length * 2);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"NetUtil.ReceiveCallback: 接收失败 {e.Message}");
            }
        }

        
        private static void Close()
        {
            
        }

        private static void OnReceiveData()
        {
            
        }
    }
}