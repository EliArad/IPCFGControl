using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class AppCommon
    {

        public enum APPErrors
        {
            STATUS_OK,
            STATUS_ERR,
            STATUS_FG_STILL_RUNNING,
            STATUS_FG_PENDING,
            STATUS_FG_TIMEOUT
        }

        public enum MODULES
        {
            MANAGER_MODULE,
            FG_MODULE,
            UDP_MODULE,
            IP_MODULE
        }
    }
}
