using System;
using System.Diagnostics;
using DLG.Log;

namespace Service
{
    public class Log : FileLog
    {
        public enum LogType
        {
            Error = 1,
            Warning = 2,
            Information = 4,
        }

        private static bool _serviceMode;
        private const string _log  = "Application";
        private const string _source = "DLG MCM"; 

        public void InitLog(bool serviceMode)
        {
            _serviceMode = serviceMode;
            if (serviceMode)
            {
                if (!EventLog.SourceExists(_source))
                {
                    EventLog.CreateEventSource(_source, _log);
                }
                return;
            }
            Console.Clear();
        }

        public static void Pause()
        {
            Console.WriteLine("Press any key to Exit!!");
            Console.ReadLine();
        }

        public static void Write(string message)
        {
            if (_serviceMode)
                EventLog.WriteEntry(_source, message);
            else
                Console.Write(message);
        }

        public static void Write(string message, LogType logtype)
        {
            if (_serviceMode)
                EventLog.WriteEntry(_source, message, (EventLogEntryType)logtype);
            else
                Console.Write(logtype + ": " + message);
        }

        public static void WriteLine(string message)
        {
            if (_serviceMode)
                EventLog.WriteEntry(_source, message);
            else
                Console.WriteLine(message);  
        }

        public static void WriteLine(string message, LogType logtype)
        {
            if (_serviceMode)
                EventLog.WriteEntry(_source, message, (EventLogEntryType)logtype);
            else
                Console.WriteLine(logtype + ": " + message);
        }
    }
}
