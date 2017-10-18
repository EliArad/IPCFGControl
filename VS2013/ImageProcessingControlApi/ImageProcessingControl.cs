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

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPStartProcess();

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int IPGetResult(out float result);

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPClose();

        [DllImport("ImageProcessingLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void IPStop();

        public AppCommon.APPErrors Start(AutoResetEvent ev)
        {
            IPStartProcess();
            return AppCommon.APPErrors.STATUS_OK;
        }

        public AppCommon.APPErrors Stop()
        {
            return AppCommon.APPErrors.STATUS_OK;
        }
    }
  
}
