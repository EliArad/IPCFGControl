﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using FrameGrabberApi;
using System.Threading;
using SLogApi;
using UdpIpcControlApi;
using ImageProcessingControlApi;


namespace ImageProcessingControlApp
{
    [Serializable]
    public struct AppConfig
    {
        public int num1;
        public int num2;
        public string IpAddress;
        public int port;
        public int FrameGrabberMaxTimeout;
        public bool logOnFile;
        public bool logOnScreen;

    }
    public class AppManager : IDisposable  
    {
        uint m_frameNumber = 1;
        AppConfig m_config;
        byte[] m_fgBuffer;
        
        FrameGrabberControl m_fgControl;
        UdpIpcControl m_udpClient = new UdpIpcControl();
        ImageProcessingControl m_ipc = new ImageProcessingControl();

        bool m_running = false;
        
        
        AutoResetEvent m_fgEvent = new AutoResetEvent(false);

        public void Configure(AppConfig config)
        {
            m_config = config;
        }
       
        public AppCommon.APPErrors Start()
        {
            m_frameNumber = 1;
            m_running = true;

            m_fgControl = new FrameGrabberControl();

            m_udpClient.Connect();

            float ipcResult = 0;
  
            while (m_running)
            {
                
                SLog.Instance().Write(AppCommon.MODULES.MANAGER_MODULE, "Start frame: " + m_frameNumber);
                if (m_fgControl.Start(m_config.num1, m_config.num2, m_fgEvent) == AppCommon.APPErrors.STATUS_FG_PENDING)
                {
                    bool b;
                    if ((b = m_fgEvent.WaitOne(m_config.FrameGrabberMaxTimeout)) == false)
                    {
                        if (m_running == false)
                            return AppCommon.APPErrors.STATUS_OK;
                        return AppCommon.APPErrors.STATUS_FG_TIMEOUT;
                    }
                    if (m_running == false)
                        return AppCommon.APPErrors.STATUS_OK;
                    m_fgBuffer = m_fgControl.RowData;
                    SLog.Instance().Write(AppCommon.MODULES.FG_MODULE, "Got row data to pass to IP size of: " + m_fgControl.BufferLength);

                    m_ipc.SetRowData(m_fgBuffer);
                    m_ipc.Start(3000, out ipcResult);

                    m_udpClient.Send(ipcResult);
                }


                m_frameNumber++;
            }

            return AppCommon.APPErrors.STATUS_OK;
        }

        public void Stop()
        {
            m_running = false;
            m_fgEvent.Set();
            if (m_fgControl != null)
                m_fgControl.Stop();
        }

        public void Dispose()  
        {  
            Dispose(true);  
        }

        ~AppManager()   
        {  
            // Finalizer calls Dispose(false)  
            Dispose(false);  
        }  
    // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_fgControl != null)
                    m_fgControl.Dispose();  
            }             
        }       

    }
}
