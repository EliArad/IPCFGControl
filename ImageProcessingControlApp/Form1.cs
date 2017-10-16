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
            
            InitDataGrid();
            SLog.Instance().Initialize("ImageProcessingApp.csv");
            LoadSettings();

            SLog.ManagerCallbackMsg p = new SLog.ManagerCallbackMsg(AppManagerCallback);
            SLog.Instance().SetCallback(p);
            
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
        void Process()
        {
            m_manager.Configure(m_appConfig);
            m_manager.Start();
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
    }
}
