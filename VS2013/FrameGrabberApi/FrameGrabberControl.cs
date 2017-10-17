using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using SLogApi;
using System.Threading;
 

namespace FrameGrabberApi
{
    
    public class FrameGrabberControl : IDisposable   
    {
        byte[] m_fgBuffer;
        int m_lastLength = 0;
        bool m_fgComplete = false;
        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ProgressCallback(IntPtr Buffer, int length);

        [DllImport("FrameGrappberLib.dll", CallingConvention = CallingConvention.Cdecl)]        
        public static extern void DoWork([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer, int num1, int num2);
      
                
        [DllImport("FrameGrappberLib.dll")]
        public static extern void FGClose();

        [DllImport("FrameGrappberLib.dll")]
        public static extern void FGStop();

        AppCommon.MODULES m_module = AppCommon.MODULES.FG_MODULE;

        bool m_running = false;
        AutoResetEvent m_event;
        
        public FrameGrabberControl()        
        {
             
        }

        public bool Stop()
        {
            bool stat = true;
            m_running = false;
            try
            {
                FGStop();
            }
            catch (Exception err)
            {
                stat = false;
            }

            return stat;
        }

        public byte [] RowData
        {
            get
            {
                return m_fgBuffer;
            }
        }
        public bool IsComplete
        {
            get
            {
                return m_fgComplete;
            }
        }
        public int BufferLength
        {
            get
            {
                return m_lastLength;
            }
        }
        public AppCommon.APPErrors Start(int num1, int num2 , AutoResetEvent ev)
        {
            m_fgComplete = false;
             
            ProgressCallback callback =
            (Buffer, length) =>
            {
                if (m_running == false)
                    return;
                //Console.WriteLine("Progress = {0}", length);

                if (m_fgBuffer == null || length != m_lastLength)
                    m_fgBuffer = new byte[length];
                Marshal.Copy(Buffer, m_fgBuffer, 0, length);
                m_lastLength = length;
                SLog.Instance().Write(m_module, "Got response");
                m_fgComplete = true;
                ev.Set();
                return;
            };

            m_running = true;
            try
            {
                DoWork(callback, num1, num2);
                SLog.Instance().Write(m_module, "Called to acquire frame");
                return AppCommon.APPErrors.STATUS_FG_PENDING;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
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
                try
                {
                    FGClose();
                }
                catch (Exception err)
                {

                }
            }
        }          
    }
    
}
