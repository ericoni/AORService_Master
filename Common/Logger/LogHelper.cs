using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Logger
{
    public static class LogHelper
    {
        private static LogBase logger = null;
        public static void Log(LogTarget target, LogService service, string message)
        {
            switch (target)
            {
                case LogTarget.File:
                    logger = new FileLogger();
                    logger.Log(service, message);
                    break;
                case LogTarget.Database:
                    logger = new DBLogger();
                    logger.Log(service, message);
                    break;
                case LogTarget.EventLog:
                    logger = new EventLogger();
                    logger.Log(service, message);
                    break;
                default:
                    return;
            }
        }
    }
}
