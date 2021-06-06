using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASL.PathTracer
{
    public enum LogType
    {
        Info,
        Warnning,
        Error,
        Complete,
        Debugging,
    }

    public class Log
    {
        public struct LogItem
        {
            public LogType logType;

            public string message;

            public LogItem(string message, LogType type)
            {
                this.message = message;
                this.logType = type;
            }

            public Color GetFontColor()
            {
                switch (logType)
                {
                    case LogType.Error:
                        return Color.maroon;
                    case LogType.Complete:
                        return Color.mediumBlue;
                    case LogType.Info:
                    case LogType.Warnning:
                    case LogType.Debugging:
                    default:
                        return Color.black;
                }
            }

            public Color GetBackColor()
            {
                switch (logType)
                {
                    case LogType.Error:
                        return Color.darkOrange;
                    case LogType.Warnning:
                        return Color.yellow;
                    case LogType.Complete:
                        return Color.green;
                    case LogType.Debugging:
                        return Color.cyan;
                    case LogType.Info:
                    default:
                        return Color.white;
                }
            }
        }

        public delegate void AddLogEventHandler(LogItem logItem);

        public delegate void ClearLogEventHandler();

        public static event AddLogEventHandler onAddLogEvent;

        public static event ClearLogEventHandler onClearLogEvent;

        private static Log sInstance;

        public static void Init()
        {
            if (sInstance == null)
                sInstance = new Log();
        }

        private Log()
        {
        }

        public static void Info(string message)
        {
            if (sInstance == null)
                return;
            sInstance.AddLogItem(LogType.Info, message);
        }

        public static void Warn(string message)
        {
            if (sInstance == null)
                return;
            sInstance.AddLogItem(LogType.Warnning, message);
        }

        public static void Err(string message)
        {
            if (sInstance == null)
                return;
            sInstance.AddLogItem(LogType.Error, message);
        }

        public static void CompleteInfo(string message)
        {
            if (sInstance == null)
                return;
            sInstance.AddLogItem(LogType.Complete, message);
        }

        public static void AddLog(LogType type, string message)
        {
            if (sInstance == null)
                return;
            sInstance.AddLogItem(type, message);
        }

        public static void Clear()
        {
            if (sInstance == null)
                return;
            onClearLogEvent();
        }

        private void AddLogItem(LogType type, string message)
        {
            LogItem item = new LogItem(message, type);

            onAddLogEvent(item);
        }

        
    }
}
