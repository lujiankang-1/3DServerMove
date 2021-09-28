using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;

namespace ServerTest
{
    class ClientState
    {
        public Socket socket;
        public Byte[] readBuff = new byte[1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;
    }

    #region 异步服务器
    //异步服务器
    //class Server
    //{
    //    static Socket listenfd;
    //    static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    //    public static void Main(string[] args)
    //    {
    //        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
    //            ProtocolType.Tcp);

    //        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
    //        IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
    //        listenfd.Bind(ipEp);

    //        listenfd.Listen(0);
    //        Console.WriteLine("[服务器]启动成功");

    //        listenfd.BeginAccept(AcceptCallback, listenfd);

    //        Console.ReadLine();
    //    }

    //    public static void AcceptCallback(IAsyncResult ar)
    //    {
    //        try
    //        {
    //            Console.WriteLine("[服务器]Accept");
    //            Socket listenfd = (Socket)ar.AsyncState;
    //            Socket clientfd = listenfd.EndAccept(ar);

    //            ClientState state = new ClientState();
    //            state.socket = clientfd;
    //            clients.Add(clientfd, state);

    //            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

    //            listenfd.BeginAccept(AcceptCallback, listenfd);
    //        }
    //        catch (SocketException ex)
    //        {
    //            Console.WriteLine("Socket Accept fail" + ex.ToString());
    //        }
    //    }

    //    public static void ReceiveCallback(IAsyncResult ar)
    //    {
    //        try
    //        {
    //            ClientState state = (ClientState)ar.AsyncState;
    //            Socket clientfd = state.socket;
    //            int count = clientfd.EndReceive(ar);

    //            if (count == 0)
    //            {
    //                clientfd.Close();
    //                clients.Remove(clientfd);
    //                Console.WriteLine("Socket Close");
    //                return;
    //            }

    //            string recvStr =
    //                System.Text.Encoding.Default.GetString(state.readBuff,0,count);

    //            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" +
    //                recvStr;
    //            byte[] sendBytes = 
    //                System.Text.Encoding.Default.GetBytes(sendStr);
    //            Console.WriteLine("[客户端]" + sendStr);
    //            foreach (ClientState s in clients.Values)
    //            {
    //                s.socket.Send(sendBytes);
    //            }
    //            //clientfd.Send(sendBytes);
    //            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
    //        }
    //        catch(SocketException ex)
    //        {
    //            Console.WriteLine("Socket Receive fail" + ex.ToString());
    //        }
    //    }
    //}
    #endregion

    #region Select服务器
    class Server
    {
        static Socket listenfd;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        public static void Main(string[] args)
        {
            listenfd = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp);

            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach(ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                Socket.Select(checkRead, null, null, 1000);
                foreach(Socket s in checkRead)
                {
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
            }
        }
        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }

        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接受
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }catch(SocketException ex)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Receive SocketException" + ex.ToString());
                return false;
            }

            //客户端关闭
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }

            //广播
            string recvStr =
                System.Text.Encoding.Default.GetString(state.readBuff,0, count);
            Console.WriteLine("Receive" + recvStr);
            string[] split = recvStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            //string sendStr = /*clientfd.RemoteEndPoint.ToString() + ":" + */recvStr;
            //byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            //foreach(ClientState s in clients.Values)
            //{
            //    s.socket.Send(sendBytes);
            //}
            return true;
        }

        public static void Send(ClientState cs,string sendStr)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            cs.socket.Send(sendBytes);
        }
    }
    #endregion
}
