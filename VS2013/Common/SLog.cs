using CommonLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLogApi
{ 

    public sealed class  SLog
    {
        
        private static SLog _instance;
        private static object syncRoot = new Object();
        string m_dateName;
        string m_logFileName;
        string m_fullLogPathName;

        private SLog()
        {
            Directory.CreateDirectory("Logs");
        }

        public delegate void ManagerCallbackMsg(int code, AppCommon.MODULES module, DateTime dime , string msg);
        ManagerCallbackMsg pMsgCallback = null;

        public static SLog Instance()
        {
            // Uses lazy initialization.
            // Note: this is not thread safe.
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new SLog();
                }
            }
            return _instance;
        }

        public void SetCallback(ManagerCallbackMsg p)
        {
            pMsgCallback = p;
        }

        public void Initialize(string logFileName)
        {
            DateTime d = DateTime.Now;
            string dname = "Logs\\" + d.Year + "_" + d.Month + "_" + d.Day;
            m_dateName = dname;
            m_logFileName = logFileName;
            m_fullLogPathName = dname + "_" + logFileName;
        }
        public void Write(AppCommon.MODULES module, string str)
        {
            lock (this)
            {

                DateTime d = DateTime.Now;
                string dname = "Logs\\" + d.Year + "_" + d.Month + "_" + d.Day;
                if (dname != m_dateName)
                {
                    m_dateName = dname;
                    m_fullLogPathName = dname + "_" + m_logFileName;
                }

                using (FileStream fs = new FileStream(m_fullLogPathName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("{0}, {1} , {2}", module.ToString(), d , str);
                }
                if (pMsgCallback != null)
                {
                    pMsgCallback(100, module, d, str);
                }
            }
        }
    }
}

 

  
