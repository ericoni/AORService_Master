using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.Logger
{
    public enum LogTarget
    {
        File, Database, EventLog
    }

    public enum LogService
    {
        NMSNetworkModel,
		NMSSmartContainer,
		SCADACrunching,
		SCADATwoPhaseCommit,
		SCADASetpoint,
		CalculationEngineDistributer,
		CETwoPhaseCommit,
		CEDataCollector,
		CalculationEngineForecast,
		SCADADataCollector,
		CalculateHourlyForecast,
		AORCache2PC
    }

    public abstract class LogBase
    {
        protected static readonly object lockObj = new object();
        public abstract void Log(LogService service, string message);
    }

    public class FileLogger : LogBase
    {
        public string folderPath = @"DERMSdirt\Logs";
        public string fileName = "IDGLog.txt";
        public override void Log(LogService service, string message)
        {
            lock (lockObj)
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(folderPath))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(folderPath);
                }              
                using (StreamWriter streamWriter = new StreamWriter(folderPath+ "\\"+ service.ToString() + fileName, true))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString() + " " + message);
                    streamWriter.Close();
                }
            }
        }
    }

    public class EventLogger : LogBase
    {
        public override void Log(LogService service, string message)
        {
            lock (lockObj)
            {
                EventLog m_EventLog = new EventLog("");
                m_EventLog.Source = "IDGEventLog";
                m_EventLog.WriteEntry(message);
            }
        }
    }

    public class DBLogger : LogBase
    {
        string connectionString = string.Empty;
        public override void Log(LogService service, string message)
        {
            lock (lockObj)
            {
                //Code to log data to the database
            }
        }
    }
}
