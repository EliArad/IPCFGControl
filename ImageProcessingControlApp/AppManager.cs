using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using FrameGrabberApi;


namespace ImageProcessingControlApp
{
    [Serializable]
    public struct AppConfig
    {
        public int num1;
        public int num2;
        public string IpAddress;
        public int port;

    }
    public class AppManager : IDisposable  
    {
        AppConfig m_config;

        FrameGrabberControl m_fgControl = new FrameGrabberControl();

       

        public void Configure(AppConfig config)
        {
            m_config = config;
        }
       
        public AppCommon.APPErrors Start()
        {

            m_fgControl.Start(m_config.num1, m_config.num2);

            return AppCommon.APPErrors.STATUS_OK;
        }

        public void Stop()
        {
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
                m_fgControl.Dispose();  
            }             
        }       

    }
}
