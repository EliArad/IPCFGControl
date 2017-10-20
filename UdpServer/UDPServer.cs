using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 


namespace UdpServerLib
{
    
    public class UDPServer
    {

        public enum UDP_MSGCB
        {
            MSG_RECIVED,
            DATA_RECIVED
        }
        int m_port;

        UdpClient m_newsock;
        IPEndPoint m_sender;
        IPEndPoint m_ipep;
        byte[] data = new byte[1024];
        bool m_running = false;
        Thread m_thread;
        AppCommon.UPayload m_uPayload = new AppCommon.UPayload();
        AppCommon.UDPMessageHeader m_uHeader = new AppCommon.UDPMessageHeader();
        public delegate void UdbMsgCallback(UDP_MSGCB code, 
                                            AppCommon.UDP_MESSAGE_CODES msgCode,
                                            byte [] buffer,
                                            int size);
        

        UdbMsgCallback pMsgCallback = null;

        public UDPServer(UdbMsgCallback msgCallback, int port = 9050)
        {
            m_port = port;
            pMsgCallback = msgCallback;
        }
        public void Connect()
        {
            try
            {
                m_ipep = new IPEndPoint(IPAddress.Any, m_port);
                m_newsock = new UdpClient(m_ipep);
                m_sender = new IPEndPoint(IPAddress.Any, 0);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        public void Stop()
        {
            m_running = false;
            if (m_newsock != null)
            {
                m_newsock.Close();
            }
            if (m_thread != null)
                m_thread.Join();
        }

        byte [] m_rbuffer = new byte[100];
         
        public virtual void Listner(ushort Destination)
        {
            while (m_running)
            {
                try
                {
                    data = m_newsock.Receive(ref m_sender);
                    pMsgCallback(UDP_MSGCB.MSG_RECIVED, AppCommon.UDP_MESSAGE_CODES.NONE, null , 0);
                    AppCommon.ByteArrayToStruct<AppCommon.UDPMessageHeader>(data, ref m_uHeader);
                    if (m_uHeader.StartCode1 == 0x1122 && m_uHeader.StartCode2 == 0x3344 && m_uHeader.Destination == Destination)
                    {                        
                        AppCommon.ByteArrayToStruct<AppCommon.UPayload>(data, ref m_uPayload);
                        Array.Copy(data, Marshal.SizeOf(m_uHeader) + 2, m_rbuffer, 0, m_uHeader.size);
                        pMsgCallback(UDP_MSGCB.DATA_RECIVED, m_uPayload.msgCodes,  m_rbuffer, m_uHeader.size);
                        /*
                        if (m_uPayload.msgCodes == AppCommon.UDP_MESSAGE_CODES.SET_SIGNAL_ID)
                        {
                            float value = BitConverter.ToSingle(m_rbuffer, 0);
                            pMsgCallback(UDP_MSGCB.DATA_RECIVED, m_uPayload.msgCodes, m_uHeader, m_uPayload, value.ToString());
                        }
                        if (m_uPayload.msgCodes == AppCommon.UDP_MESSAGE_CODES.START_FECTH_ACK)
                        {
                            pMsgCallback(UDP_MSGCB.DATA_RECIVED,m_uPayload.msgCodes,  m_uHeader, m_uPayload, string.Empty);
                        }
                        */
                    }                     
                }
                catch (Exception)
                {
                    if (m_running == false)
                        return;
                }
            }
        }
        public void Start(ushort Destination)
        {
            if (m_running == false)
            {
                m_running = true;

                m_thread = new Thread(() => Listner(Destination));
                m_thread.Start();
            }           
        }
    }
}
