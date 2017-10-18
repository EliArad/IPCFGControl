using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpIpcControlApi
{
    public class UdpIpcControl
    {


        AppCommon.UPayload m_umsg = new AppCommon.UPayload();
        UdpClient m_udpClient = new UdpClient();
         
        public void Connect(string IpAddress, int port)
        {
            m_udpClient.Connect(IpAddress, port);
        }
        public void Connect()
        {
            try
            {
                m_udpClient.Connect("localhost", 9050);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public void Send(byte[] data, int size)
        {
            try
            {
                m_udpClient.Send(data, size);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public void Send(byte[] data)
        {
            try
            {
                Send(data, data.Length);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public void Send(float data)
        {
            m_umsg.header.opcode = 1;
            m_umsg.header.size = 4;            
            m_umsg.data = data;
            m_umsg.crc = 1234;
            byte [] b = AppCommon.StructToByteArray<AppCommon.UPayload>(m_umsg);            
            
            Send(b);
            
        }

        public void Send(string msg)
        {
            try
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
                m_udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public void Close()
        {

            m_udpClient.Close();
        }

        void Receiver()
        {
            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = m_udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            // Uses the IPEndPoint object to determine which of these two hosts responded.
            Console.WriteLine("This is the message you received " +
                                         returnData.ToString());
            Console.WriteLine("This message was sent from " +
                                        RemoteIpEndPoint.Address.ToString() +
                                        " on their port number " +
                                        RemoteIpEndPoint.Port.ToString());
        }

    }
}
