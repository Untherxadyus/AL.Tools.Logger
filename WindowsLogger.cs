using AL.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Tools.Logger
{
    public static class WindowsLogger
    {
        private static EventLog ApplicationLog = new EventLog();

        static WindowsLogger()
        {
            var Source = Settings.AppSettings("AL-LogSource");
            var Log = Settings.AppSettings("AL-LogName");

            if (EventLog.SourceExists(Source) == false)
                EventLog.CreateEventSource(Source, Log);

            ApplicationLog.Source = Source;
            ApplicationLog.Log = Log;
        }

        /// <summary>
        /// Adds an entry to the windows log.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="LogType"></param>
        public static void AddEntry(string Message, EventLogEntryType LogType = EventLogEntryType.Information)
        {
            ApplicationLog.WriteEntry(Message, LogType);
        }
    }
}
