using CommonLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UdpIpcControlApi;
using UdpServerLib;


namespace UdpServerApp
{
    public partial class Form1 : Form
    {
        UDPServer m_server;
        UdpIpcControl m_udpClient;
        Dictionary<AppCommon.UDP_MESSAGE_CODES, Func<byte[], int, int>> m_callbackFunctions;

        
        public Form1()
        {
            InitializeComponent();
            lblStatus.Text = string.Empty;

            // The second parameter in the Func is the return value
            //Func<AppCommon.UPayload, int> FunctionPTR = StartFecthAck;
            m_callbackFunctions = new Dictionary<AppCommon.UDP_MESSAGE_CODES, Func<byte [] , int , int>>();
            m_callbackFunctions.Add(AppCommon.UDP_MESSAGE_CODES.START_FECTH_ACK, StartFecthAck);
            m_callbackFunctions.Add(AppCommon.UDP_MESSAGE_CODES.SET_SIGNAL_ID, SetSignalId);


            try
            {
                UDPServer.UdbMsgCallback p = new UDPServer.UdbMsgCallback(MsgCallback);
                m_server = new UDPServer(p , 3001);
                m_server.Connect();
                m_server.Start(0x8888);
                m_udpClient = new UdpIpcControl(0x5555);
                m_udpClient.Connect(2001);
                lblStatus.Text = "Start listening";
                lblDate.Text = string.Empty;
                Control.CheckForIllegalCrossThreadCalls = false;
            }
            catch (Exception err)
            {
                lblStatus.Text = err.Message;
            }
        }
        int StartFecthAck(byte [] payloadData, int size)
        {
            return 1;
        }
        int SetSignalId(byte[] payloadData, int size)
        {
            float value = BitConverter.ToSingle(payloadData, 0);
            lblStatus.Text = "SetSignalId: " + value;
            textBox1.AppendText(lblDate.Text + "  SetSignalId: " + value +  Environment.NewLine);
            return 1;
        }

        void MsgCallback(UdpServerLib.UDPServer.UDP_MSGCB code,
                         AppCommon.UDP_MESSAGE_CODES msgCode,
                         byte [] buffer,
                         int size)
        {
            switch (code)
            {

                case UDPServer.UDP_MSGCB.MSG_RECIVED:
                {
                    
                    lblDate.Text = DateTime.Now.ToString();
                    lblStatus.Text =  "Msg Received: ";
                }
                break;
                case UDPServer.UDP_MSGCB.DATA_RECIVED:
                {
                    Func<byte[], int, int> FunctionPTR = m_callbackFunctions[msgCode];
                    FunctionPTR(buffer, size);

                    textBox1.AppendText(lblDate.Text + "  Sending ack" + Environment.NewLine);
                    m_udpClient.Cmd.SendAck();
                }
                break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_server.Stop();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
