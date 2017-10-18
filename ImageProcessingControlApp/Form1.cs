using CommonLib;
using Newtonsoft.Json;
using SLogApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessingControlApp
{
    public partial class Form1 : Form
    {
        AppConfig m_appConfig;
        const string ConfigFileName = "ImageProcessingConfig.json";

        Thread m_managerThread;


        AppManager m_manager = new AppManager();
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            InitDataGrid();
            SLog.Instance().Initialize("ImageProcessingApp.csv");
            LoadSettings();

            SLog.ManagerCallbackMsg p = new SLog.ManagerCallbackMsg(AppManagerCallback);
            SLog.Instance().SetCallback(p);

            this.Height = this.Height - dataGridView1.Height - 12;
            
        }

        void EnableControl(Control control,  bool Enable)
        {
            if (dataGridView1.InvokeRequired)
            {
                control.BeginInvoke((MethodInvoker)delegate()
                {
                    control.Enabled = Enable;

                });
            }
            else
            {
                control.Enabled = Enable;
            }
        }

        void InsertToDataGrid(AppCommon.MODULES module, DateTime dtime, string msg)
        {
            dataGridView1.Rows.Add();
            int index = dataGridView1.RowCount - 1;
            dataGridView1.Rows[index].Cells[0].Value = module.ToString();
            dataGridView1.Rows[index].Cells[1].Value = dtime.ToString();
            dataGridView1.Rows[index].Cells[2].Value = msg;

            
        }
        void AppManagerCallback(int code, AppCommon.MODULES module, DateTime dtime, string msg)
        {
            lock (this)
            {
                switch (code)
                {
                    case 100:
                        {

                            if (m_appConfig.logOnScreen == false)
                                return;
                            if (dataGridView1.InvokeRequired)
                            {
                                dataGridView1.BeginInvoke((MethodInvoker)delegate()
                                {
                                    InsertToDataGrid(module, dtime, msg);

                                });
                            }
                            else
                            {
                                InsertToDataGrid(module, dtime, msg);
                            }
                        }
                        break;
                }
            }
        }

        void InitDataGrid()
        {
            int cind = 0;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[cind].Name = "Module";
            dataGridView1.Columns[cind++].Width = 80;

            dataGridView1.Columns[cind].Name = "DateTime";
            dataGridView1.Columns[cind++].Width = 140;

            dataGridView1.Columns[cind].Name = "Status";
            dataGridView1.Columns[cind++].Width = 300;

        }

        void SaveSettings()
        {
            try
            {

                m_appConfig.IpAddress = txtIpAddress.Text;
                int.TryParse(txtNum1.Text, out m_appConfig.num1);
                int.TryParse(txtNum2.Text, out m_appConfig.num2);
                int.TryParse(txtPort.Text, out m_appConfig.port);
                m_appConfig.logOnFile = chkOnFile.Checked;
                m_appConfig.logOnScreen = chkOnScreen.Checked;

                using (StreamWriter file = File.CreateText(ConfigFileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, m_appConfig);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to save settings: " + err.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "+")
            {
                button1.Text = "-";
                this.Height = this.Height + dataGridView1.Height + 12;
            }
            else
            {
                button1.Text = "+";
                this.Height = this.Height - dataGridView1.Height - 12;
            }
        }

        void LoadSettings()
        {

            try
            {
                if (File.Exists(ConfigFileName) == false)
                    return;

                using (StreamReader file = File.OpenText(ConfigFileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    m_appConfig = (AppConfig)serializer.Deserialize(file, typeof(AppConfig));
                }

                txtIpAddress.Text = m_appConfig.IpAddress;
                txtNum1.Text = m_appConfig.num1.ToString();
                txtNum2.Text = m_appConfig.num2.ToString();
                txtPort.Text = m_appConfig.port.ToString();
                m_appConfig.FrameGrabberMaxTimeout = 10000;
                chkOnFile.Checked = m_appConfig.logOnFile;
                chkOnScreen.Checked = m_appConfig.logOnScreen;

                SLog.Instance().EnableLog(m_appConfig.logOnFile);

            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_managerThread == null || m_managerThread.IsAlive == false)
            {
                m_managerThread = new Thread(Process);
                m_managerThread.Start();
            }
            else
            {
                MessageBox.Show("Still running");
            }
        }
        
        void GuiStart(bool start)
        {
            if (start == true)
            {
                EnableControl(btnStart, false);
                dataGridView1.Rows.Clear();                
            }
            else
            {
                EnableControl(btnStart, true);
            }
        }
        void ShowErrors(AppCommon.APPErrors status)
        {
            switch (status)
            {
                case AppCommon.APPErrors.STATUS_ERR:
                    MessageBox.Show("Yet , unknown error");
                break;
                case AppCommon.APPErrors.STATUS_FG_STILL_RUNNING:
                {
                    MessageBox.Show("Frame grabber still running");
                }
                break;
                case AppCommon.APPErrors.STATUS_FG_TIMEOUT:
                {
                    MessageBox.Show("Frame grabber timeout");
                }
                break;
        }

        }
        void Process()
        {
            try
            {
                m_manager.Configure(m_appConfig);
                GuiStart(true);
                ShowErrors(m_manager.Start());
                GuiStart(false);

            }
            catch (Exception err)
            {             
                MessageBox.Show(err.Message);
                GuiStart(false);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        void Stop()
        {
            m_manager.Stop();
            if (m_managerThread != null)
            {
                m_managerThread.Join();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            Stop();
            m_manager.Dispose();
        }

        private void chkOnFile_CheckedChanged(object sender, EventArgs e)
        {
            m_appConfig.logOnFile = chkOnFile.Checked;
            SLog.Instance().EnableLog(m_appConfig.logOnFile);
        }

        private void chkOnScreen_CheckedChanged(object sender, EventArgs e)
        {
            m_appConfig.logOnScreen = chkOnScreen.Checked;
                       
        }
    }
}
