using System;
using System.Collections.Generic;
using System.Text;

namespace ServerTest
{
    class EventHandler
    {
        public static void OnDisconnect(ClientState c)
        {
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ',';
            Console.WriteLine(sendStr);
            foreach(ClientState cs in Server.clients.Values)
            {
                Server.Send(cs, sendStr);
            }
        }
    }
}
