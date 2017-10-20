using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UdpServerLib;

namespace UdpIpcControlApi
{
    public class Commands 
    {
        AppCommon.UPayload m_umsg = new AppCommon.UPayload();

        UdpClient m_client = null;


        ushort m_destination;

        public Commands(UdpClient client, ushort destination)
        {
            m_client = client;
            m_destination = destination;
        }
  
        public void Send(byte[] data, int size)
        {
            try
            {
                if (m_client.Send(data, size) != size)
                {
                    throw (new SystemException("Failed to send client data to server of: " + size + " bytes"));
                }
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

        void SetHeader()
        {
            m_umsg.header.StartCode1 = 0x1122;
            m_umsg.header.StartCode2 = 0x3344;
            m_umsg.header.Destination = m_destination;
        }
        public void Send(AppCommon.UDP_MESSAGE_CODES code, ushort[] data)
        {

        }

        public void Send(float data)
        {
            SetHeader();
            m_umsg.header.size = sizeof(float);

            ushort [] floatData = new ushort[2];

            byte[] allBytes = BitConverter.GetBytes(data);
            Buffer.BlockCopy(allBytes, 0, floatData, 0, allBytes.Length);
            m_umsg.msgCodes = AppCommon.UDP_MESSAGE_CODES.SET_SIGNAL_ID;

            m_umsg.header.checksum = AppCommon.CalcUdpChecksum(floatData);
            byte[] buf = new byte[Marshal.SizeOf(m_umsg) + m_umsg.header.size];
            Array.Copy(AppCommon.StructToByteArray<AppCommon.UPayload>(m_umsg), buf, Marshal.SizeOf(m_umsg));


            Array.Copy(allBytes , 0 , buf , Marshal.SizeOf(m_umsg),  4);

            Send(buf);
        }

        public void SendAck()
        {
            SetHeader();
            m_umsg.header.size = 0;

            m_umsg.header.checksum = 0;
            m_umsg.msgCodes = AppCommon.UDP_MESSAGE_CODES.START_FECTH_ACK;
            byte[] buf = new byte[Marshal.SizeOf(m_umsg) + m_umsg.header.size];
            Array.Copy(AppCommon.StructToByteArray<AppCommon.UPayload>(m_umsg), buf, Marshal.SizeOf(m_umsg));

            Send(buf);
        }

        public void Send(string msg)
        {
            try
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);
                Send(sendBytes, sendBytes.Length);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
