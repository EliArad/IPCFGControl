using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using SLogApi;
 

namespace FrameGrabberApi
{
    
    public class FrameGrabberControl : IDisposable   
    {
      
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate string GetFilePathCallback(string filter);

        [DllImport("FrameGrappberLib.dll", CallingConvention = CallingConvention.Cdecl)]        
        public static extern void DoWork([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer, int num1, int num2);

        


                
        [DllImport("FrameGrappberLib.dll")]
        public static extern void FGClose();

        [DllImport("FrameGrappberLib.dll")]
        public static extern void FGStop();

        AppCommon.MODULES m_module = AppCommon.MODULES.FG_MODULE;

        bool m_running = false;
        
        public FrameGrabberControl()        
        {
             
        }

        public void Stop()
        {
            m_running = false;
            FGStop();
        }

        public AppCommon.APPErrors Start(int num1, int num2)
        {

            ProgressCallback callback =
            (value) =>
            {
                if (m_running == false)
                    return;
                Console.WriteLine("Progress = {0}", value);
                SLog.Instance().Write(m_module, "Got response");
                return;
            };

            m_running = true;

            DoWork(callback, num1, num2);
            SLog.Instance().Write(m_module, "Called to acquire frame");
            return AppCommon.APPErrors.STATUS_FG_PENDING;
        }

        public void Dispose()  
        {  
            Dispose(true);  
        }

        ~FrameGrabberControl()   
        {  
            // Finalizer calls Dispose(false)  
            Dispose(false);  
        }  
    // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                FGClose();
            }
        }          
    }
    
}
