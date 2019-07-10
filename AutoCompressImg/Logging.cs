using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompressImg
{
    public class Logging
    {

        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Fatal(string strMsg)
        {
            log.Fatal(strMsg);
        }

        public static void Fatal(Exception e)
        {
            log.Fatal(e.ToString());
        }

        public static void Error(string strMsg)
        {
            log.Error(strMsg);
        }

        public static void Error(Exception e)
        {
            log.Error(e.ToString());
        }

        public static void Warn(string strMsg)
        {
            log.Error(strMsg);
        }

        public static void Warn(Exception e)
        {
            log.Error(e.ToString());
        }

        public static void Debug(string strMsg)
        {
            log.Error(strMsg);
        }

        public static void Debug(Exception e)
        {
            log.Error(e.ToString());
        }

        public static void Info(string strMsg)
        {
            log.Error(strMsg);
        }

        public static void Info(Exception e)
        {
            log.Error(e.ToString());
        }
    }

}
