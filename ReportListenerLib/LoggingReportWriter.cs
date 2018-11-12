using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Common;

namespace ReportListenerLib
{
    class LoggingReportWriter : IReportListenerWriter
    {
        public bool Init(params object[] paramList) { IsInitialized = true; return true; }
        public bool StartReportWriter() 
        { 
            IsWriting = IsInitialized;
            if (IsWriting && (m_infoEventHandler != null))
                m_infoEventHandler(null, new LoggingEventArgs("LoggingReportWriter", "Logging report writer started"));
            return IsWriting; 
        }
        public bool StopReportWriter()
        {
            IsWriting = false;
            if (m_writerStoppedHandler != null)
                m_writerStoppedHandler(null, new ServiceStoppedEventArgs("LoggingReportWriter", "Logging report writer stopped"));
            else if (m_infoEventHandler != null)
                m_infoEventHandler(null, new LoggingEventArgs("LoggingReportWriter", "Logging report writer stopped"));
            return true;
        }

        public bool IsInitialized { get; private set; }
        public bool IsWriting { get; private set; }

        public bool HadError { get; private set; }
        public string LastErrorMessage { get; private set; }

        #region Events
        private event LoggingEventHandler m_infoEventHandler;
        private event LoggingEventHandler m_errorEventHandler;
        private event ServiceStoppedEventHandler m_writerStoppedHandler;

        public event LoggingEventHandler OnError
        {
            add { m_errorEventHandler += value; }
            remove { m_errorEventHandler -= value; }
        }
        public event LoggingEventHandler OnInfo
        {
            add { m_infoEventHandler += value; }
            remove { m_infoEventHandler -= value; }
        }
        public event ServiceStoppedEventHandler OnWriterStopped
        {
            add { m_writerStoppedHandler += value; }
            remove { m_writerStoppedHandler -= value; }
        }
        #endregion

        // returns 0 on success, 1 if report is a duplicate, 4 on an error
        public int TakeReport(string xml)
        {
            int returnCode = 4;
            if (IsWriting)
            {
                try
                {
                    if (m_infoEventHandler != null)
                    {
                        m_infoEventHandler(null, new LoggingEventArgs("LoggingReportWriter", xml));
                        returnCode = 0;
                    }
                }
                catch (Exception ex)
                {
                    if (m_errorEventHandler != null)
                    {
                        m_errorEventHandler(null, new LoggingEventArgs("LoggingReportWriter", "Error in logger", ex));
                    }
                }
            }
            return returnCode;
        }

        public void Dispose() { }
    }
}
