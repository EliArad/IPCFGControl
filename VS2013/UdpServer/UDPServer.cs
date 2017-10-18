using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpServer
{
    public class UDPServer
    {

        int m_port;

        UdpClient m_newsock;

        public UDPServer(int port = 9050)
        {

            m_port = port;
        }
     
        public void Start()
        {

            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, m_port);
            UdpClient m_newsock = new UdpClient(ipep);
         
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            data = m_newsock.Receive(ref sender);

            Console.WriteLine("Message received from {0}:", sender.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            m_newsock.Send(data, data.Length, sender);
        }
    }
}
