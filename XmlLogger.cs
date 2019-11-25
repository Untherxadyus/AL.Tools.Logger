using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AL.Tools.Logger
{
    /// <summary>
    /// Allows to generate a XML log file and add entries to it.
    /// </summary>
    public static class XmlLogger
    {
        //Locker Variable
        private static readonly object Locker = new object();

        /// <summary>
        /// Current log path.
        /// </summary>
        public static string CurrentLog { get; private set; }

        static XmlLogger()
        {
            GetCurrentLog();
        }

        /// <summary>
        /// Adds a simple entry to the log.
        /// </summary>
        /// <param name="Message"></param>
        public static void AddEntry(string Message)
        {
            lock (Locker)
            {
                //Verify Log Exists
                GetCurrentLog();

                //Load current XML
                var doc = new XmlDocument();
                doc.Load(CurrentLog);

                var node = doc.CreateNode(XmlNodeType.Element, "log_entry", null);
                var attr = doc.CreateAttribute("create_date");
                attr.InnerText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                node.Attributes.Append(attr);
                node.InnerText = Message;

                doc.DocumentElement.AppendChild(node);

                doc.Save(CurrentLog);
            }
        }

        public static void AddEntry(Exception ex)
        {
            AddEntry(ex.Message);
        }

        public static void AddEntries(IEnumerable<string> Logs)
        {
            lock (Locker)
            {
                //Verify Log Exists
                GetCurrentLog();

                //Load current XML
                var doc = new XmlDocument();
                doc.Load(CurrentLog);

                foreach (var Message in Logs)
                {
                    var node = doc.CreateNode(XmlNodeType.Element, "log_entry", null);
                    var attr = doc.CreateAttribute("create_date");
                    attr.InnerText = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    node.Attributes.Append(attr);
                    node.InnerText = Message;

                    doc.DocumentElement.AppendChild(node);
                }

                doc.Save(CurrentLog);
            }
        }

        /// <summary>
        /// Method to retreive Current Log.
        /// </summary>
        private static void GetCurrentLog()
        {
            //Validate if current log exist
            if (CurrentLog.IsNotEmpty() && File.Exists(CurrentLog))
                return;

            //Get Log Name, get it from the config file
            var LogName = Settings.AppSettings("AL-LogName");

            //If no name was found, them create a temporary Log Name
            if (LogName.IsEmpty())
            {
                var date = DateTime.Now;
                LogName = $"TEMP-{date.ToString("yy")}{new JulianCalendar().GetDayOfYear(date)}";
            }

            var LogFile = $"{Settings.AssemblyPath}{LogName}.xml";

            //If log doesn't exists, then we create it
            if (File.Exists(LogFile) == false)
                using (var writer = XmlWriter.Create(LogFile))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Log");
                    writer.WriteEndElement();
                }

            CurrentLog = LogFile;
        }

        
    }
}
