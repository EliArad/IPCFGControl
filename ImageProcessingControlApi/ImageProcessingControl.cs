using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageProcessingControlApi
{
    public class ImageProcessingControl
    {
        AutoResetEvent m_sleep = new AutoResetEvent(false);

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPStartProcess();

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int IPGetResult(out float result);

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPClose();

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPStop();

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPSetRowData(byte [] data, int size);

        public AppCommon.APPErrors Start(int timeOut, out float result)
        {
            result = 0;
            IPStartProcess();
            m_sleep.WaitOne(timeOut);

            IPGetResult(out result);

            return AppCommon.APPErrors.STATUS_OK;
        }

        public void SetRowData(byte [] rowData)
        {
            IPSetRowData(rowData, rowData.Length);
        }
        public AppCommon.APPErrors Stop()
        {

            m_sleep.Set();
            return AppCommon.APPErrors.STATUS_OK;
        }
    }
  
}
