using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;//需要引用socket命名空间
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class NetManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (MsgListener.Count <= 0)
        {
            return;
        }
        string msgStr = MsgListener[0];
        MsgListener.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        Debug.Log("事件名：" + msgName + "   事件参数：" + msgArgs);
        EventDispatcher.instance.Dispatch(msgName, msgArgs);
    }

    

    /// 连接服务器
    /// </summary>
    static Socket socket_client;
    static string receStr;
    static byte[] readBuffer = new byte[1024];
    static List<string> MsgListener = new List<string>();
    public static void ConnectServer()
    {
        try
        {
            IPAddress pAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint pEndPoint = new IPEndPoint(pAddress, 8888);
            socket_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_client.BeginConnect(pAddress, 8888, ConnectCallback, socket_client);
            
        }
        catch (System.Exception)
        {

            Debug.Log("IP端口号错误或者服务器未开启");
        }
    }

    public static string GetDesc()
    {
        return socket_client.LocalEndPoint.ToString();
    }

    public static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("连接成功");
            NetManager.Send("Enter|" + NetManager.GetDesc() + ",0,0,0,0");
            
            //创建线程，执行读取服务器消息
            Thread c_thread = new Thread(Received);
            c_thread.IsBackground = true;
            c_thread.Start();

        }
        catch (SocketException ex)
        {
            Debug.Log("Connect Fail" + ex.ToString());
        }
    }

    /// <summary>
    /// 读取服务器消息
    /// </summary>
    public static void Received()
    {
        while (true)
        {
            try
            {
                if (socket_client.Connected == false) return;
                if (socket_client.Available <= 0) continue;
                socket_client.BeginReceive(readBuffer, 0, 1024, 0, ReceiveCallback, socket_client);
            }
            catch (System.Exception)
            {
                throw;
            }

        }
    }
    
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            receStr = Encoding.Default.GetString(readBuffer, 0, count);
            MsgListener.Add(receStr);
            Debug.Log("客户端打印服务器返回消息："+MsgListener[0]);
            //Debug.Log("客户端打印服务器返回消息：" + socket_client.RemoteEndPoint + ":" + receStr);
        }
        catch (SocketException ex)
        {
            Debug.Log("Receive Fail" + ex.ToString());
        }
    }

    
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg"></param>
    public static void Send(string msg)
    {
        try
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(msg);
            socket_client.Send(buffer);
        }
        catch (System.Exception)
        {

            Debug.Log("未连接");
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void close()
    {
        try
        {
            socket_client.Close();
            Debug.Log("关闭客户端连接");
            //SceneManager.LoadScene("control");
        }
        catch (System.Exception)
        {
            Debug.Log("未连接");
        }
    }
}
