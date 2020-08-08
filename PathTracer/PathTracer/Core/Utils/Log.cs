using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private static Log sInstance;

        private ListView m_ListView;

        public static void Init(ListView listView)
        {
            if (sInstance == null)
                sInstance = new Log(listView);
        }

        private Log(ListView listView)
        {
            m_ListView = listView;
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
            sInstance.m_ListView.Items.Clear();
        }

        private void AddLogItem(LogType type, string message)
        {
            ListViewItem item = new ListViewItem();
            item.Text = type.ToString();
            item.SubItems.Add(message);
            item.ForeColor = GetFontColor(type);
            item.BackColor = GetBackColor(type);

            this.m_ListView.Items.Add(item);
        }

        private System.Drawing.Color GetFontColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return System.Drawing.Color.Maroon;
                case LogType.Complete:
                    return System.Drawing.Color.MediumBlue;
                case LogType.Info:
                case LogType.Warnning:
                case LogType.Debugging:
                default:
                    return System.Drawing.Color.Black;
            }
        }

        private System.Drawing.Color GetBackColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return System.Drawing.Color.DarkOrange;
                case LogType.Warnning:
                    return System.Drawing.Color.Yellow;
                case LogType.Complete:
                    return System.Drawing.Color.Green;
                case LogType.Debugging:
                    return System.Drawing.Color.Cyan;
                case LogType.Info:
                default:
                    return System.Drawing.Color.White;
            }
        }
    }
}
