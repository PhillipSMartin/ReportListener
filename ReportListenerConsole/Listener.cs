using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gargoyle.Utilities.CommandLine;
using LoggingUtilitiesLib;
using GargoyleTaskLib;
using log4net;
using ReportListenerLib;
using Gargoyle.Utils.DBAccess;
using Gargoyle.Common;

namespace ReportListenerConsole
{
    public class Listener : IDisposable
    {
        CommandLineParameters m_parms;
        bool m_bTaskStarted;
        bool m_bTaskFailed;
        bool m_bWaiting;
        bool m_bReaderStarted;
        bool m_bWriterStarted;

        private ILog m_logger = LogManager.GetLogger(typeof(Listener));
        private System.Data.SqlClient.SqlConnection m_hugoConnection1;
        private System.Data.SqlClient.SqlConnection m_hugoConnection2;
        private System.Data.SqlClient.SqlConnection m_configDbConnection;

        private IReportListenerReader m_reader;
        private IReportListenerWriter m_writer;

        DateTime m_stopTime;
        private AutoResetEvent m_waitHandle = new AutoResetEvent(false);
 
        private int m_reportsRead;
        private int m_reportsWritten;
        private int m_duplicatesIgnored;
        private int m_errors;

        private bool m_inReplayMode;
        private int m_reportsReplayed;
        private int m_reportsWrittenOnReplay;

        private string m_completionMessage;
        private string m_lastErrorMessage;

        #region Housekeeping
        public Listener(CommandLineParameters parms)
        {
            m_parms = parms;
        }

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (m_bWaiting)
            {
                m_waitHandle.Set();
                System.Threading.Thread.Sleep(m_parms.HugoTimeout * 2);
            }

            if (m_bTaskStarted)
                m_bTaskStarted = !EndTask(m_parms.GetTaskName(), !m_bTaskFailed);

            if (disposing)
            {
                if (m_waitHandle != null)
                {
                    m_waitHandle.Dispose();
                    m_waitHandle = null;
                }
                if (m_reader != null)
                {
                    m_reader.Dispose();
                    m_reader = null;
                }
                if (m_writer != null)
                {
                    m_writer.Dispose();
                    m_writer = null;
                }

                if (m_hugoConnection1 != null)
                {
                    m_hugoConnection1.Dispose();
                    m_hugoConnection1 = null;
                }
                if (m_hugoConnection2 != null)
                {
                    m_hugoConnection2.Dispose();
                    m_hugoConnection2 = null;
                }
                 if (m_configDbConnection != null)
                {
                    m_configDbConnection.Dispose();
                    m_configDbConnection = null;
                }

                LoggingUtilities.OnInfo -= new LoggingEventHandler(Utilities_OnInfo);
                LoggingUtilities.OnError -= new LoggingEventHandler(Utilities_OnError);
                TaskUtilities.OnInfo -= new LoggingEventHandler(Utilities_OnInfo);
                TaskUtilities.OnError -= new LoggingEventHandler(Utilities_OnError);
            }
        }
        #endregion
        #endregion

        public bool Run()
        {
            m_bTaskFailed = true;
            try
            {
                // initialize logging
                log4net.Config.XmlConfigurator.Configure();
                LoggingUtilities.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                LoggingUtilities.OnError += new LoggingEventHandler(Utilities_OnError);
                TaskUtilities.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                TaskUtilities.OnError += new LoggingEventHandler(Utilities_OnError);

                TimeSpan stopTime;
                if (!m_parms.GetStopTime(out stopTime))
                {
                    LogInfoMessage("Invalid stop time specified", true);
                    return true;
                }

                m_stopTime = DateTime.Today + stopTime;
                if (Initialize())
                {
                    int rc = StartTask(m_parms.GetTaskName());
                    if (rc == 1)
                    {
                        LogInfoMessage("Task not started because it is a holiday");
                    }
                    else
                    {
                        // if task wasn't started (which probably means taskname was not in the table), so be it - no need to abort
                        m_bTaskStarted = (rc == 0);

                        if (StartListener())
                        {
                            m_bTaskFailed = false;  // set up was successful - we now wait for the timer to expire or for a post from an event handler
                            m_bWaiting = true;
                            bool timedOut = WaitForCompletion();
                            m_bWaiting = false;

                            m_inReplayMode = true;
                            StopListener();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_bTaskFailed = true;
                LogErrorMessage("Error in Run method", ex, true);
            }
            finally
            {
                if (m_bTaskStarted)
                {
                    m_completionMessage = String.Format("Wrote {0} out of {1} reports, {2} duplicates ignored, {3} errors - {4} out of {5} written on replay",
                        m_reportsWritten, m_reportsRead, m_duplicatesIgnored, m_errors, m_reportsWrittenOnReplay, m_reportsReplayed);
                    m_bTaskStarted = !EndTask(m_parms.GetTaskName(), !m_bTaskFailed);
                }
            }
            return !m_bTaskFailed;
        }

        #region Logging
        // events from LoggingUtilities and TaskUtilities
        private void Utilities_OnInfo(object sender, LoggingEventArgs eventArgs)
        {
            LogInfoMessage(eventArgs.Message);
        }

        private void Utilities_OnError(object sender, LoggingEventArgs eventArgs)
        {
            LogErrorMessage(eventArgs.Message, eventArgs.Exception);
        }

        // helper methods to write to log
        private void LogInfoMessage(string msg, bool updateLastErrorMsg = false)
        {
            if (updateLastErrorMsg)
                m_lastErrorMessage = msg;
            if (m_logger != null)
            {
                lock (m_logger)
                {
                    m_logger.Info(msg);
                }
            }

        }
        private void LogErrorMessage(string msg, Exception e, bool updateLastErrorMsg = false)
        {
            if (updateLastErrorMsg && e != null)
                m_lastErrorMessage = msg + "->" + e.Message;
            else
                m_lastErrorMessage = msg;

            if (m_logger != null)
            {
                lock (m_logger)
                {
                    m_logger.Error(msg, e);
                }
            }
        }
        #endregion

        #region Event handlers
        private void ReportListenerLib_OnReport(object sender, ReportEventArgs e)
        {
            if (m_inReplayMode)
                System.Threading.Interlocked.Increment(ref m_reportsReplayed);
            else
                System.Threading.Interlocked.Increment(ref m_reportsRead);

            int returnCode = m_writer.TakeReport(e.Report);

            if (m_inReplayMode)
            {
                if (returnCode == 0)
                    System.Threading.Interlocked.Increment(ref m_reportsWrittenOnReplay);
            }
            else
            {
                if (returnCode == 0)
                    System.Threading.Interlocked.Increment(ref m_reportsWritten);
                else if (returnCode == 1)
                    System.Threading.Interlocked.Increment(ref m_duplicatesIgnored);
                else
                    System.Threading.Interlocked.Increment(ref m_errors);
            }
        }
        private void ReportListenerLib_OnReaderStopped(object sender, ServiceStoppedEventArgs e)
        {
            LogInfoMessage(e.Message);

            m_bReaderStarted = false;
            if (m_reader.HadError)
            {
                m_bTaskFailed = true;
                m_lastErrorMessage = m_reader.LastErrorMessage;
            }
            if (m_bWaiting)
                m_waitHandle.Set();
        }
        private void ReportListenerLib_OnWriterStopped(object sender, EventArgs e)
        {
            m_bWriterStarted = false;
            if (m_writer.HadError)
            {
                m_bTaskFailed = true;
                m_lastErrorMessage = m_writer.LastErrorMessage;
            }
            if (m_bWaiting)
                m_waitHandle.Set();
        }
        #endregion

        #region Private Methods

        #region Task Management
        // returns 0 if task is successfully started
        // returns 1 if we should not start the task because it should not be run today (because this task should not run on a holiday, for example)
        // returns 4 if we cannot find the task name
        // returns some number higher than 4 on an unexpected failure
        private int StartTask(string taskName)
        {
            try
            {
                using (TaskUtilities taskUtilities = new TaskUtilities(m_hugoConnection1, m_parms.HugoTimeout))
                {
                    return taskUtilities.TaskStarted(taskName, null);
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to start task", ex);
                return 16;
            }
        }

        private bool EndTask(string taskName, bool succeeded)
        {
            try
            {
                using (TaskUtilities taskUtilities = new TaskUtilities(m_hugoConnection1, m_parms.HugoTimeout))
                {
                    if (succeeded)
                        return (0 != taskUtilities.TaskCompleted(taskName, m_completionMessage));
                    else
                        return (0 != taskUtilities.TaskFailed(taskName, m_lastErrorMessage));
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to stop task", ex);
                return false;
            }
        }
        #endregion

        #region Initialize
        private bool Initialize()
        {
            if (!GetDatabaseConnections())
                return false;

            if (!SelectReader(m_parms.Reader))
                return false;

            if (!SelectWriter(m_parms.Writer))
                return false;

            return true;
        }
        private bool GetDatabaseConnections()
        {
            DBAccess dbAccess = DBAccess.GetDBAccessOfTheCurrentUser(m_parms.ProgramName);
            m_hugoConnection1 = dbAccess.GetConnection("Hugo");
            if (m_hugoConnection1 == null)
            {
                LogInfoMessage("Unable to connect to Hugo", true);
                return false;
            }
            m_hugoConnection2 = dbAccess.GetConnection("Hugo");
            if (m_hugoConnection2 == null)
            {
                LogInfoMessage("Unable to connect to Hugo", true);
                return false;
            }

            m_configDbConnection = dbAccess.GetConnection("ConfigDB");
            if (m_configDbConnection == null)
            {
                LogInfoMessage("Unable to connect to ConfigDb", true);
                return false;
            }
            return true;
        }
        private bool SelectReader(string reader)
        {
            try
            {
                m_reader = ReaderFactory.GetReader(reader);
                if (m_reader == null)
                {
                    LogInfoMessage(String.Format("Unable to instantiate report reader {0}", reader), true);
                    return false;
                }
                else
                {
                    m_reader.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                    m_reader.OnError += new LoggingEventHandler(Utilities_OnError);
                    m_reader.OnReaderStopped += new ServiceStoppedEventHandler(ReportListenerLib_OnReaderStopped);
                    m_reader.OnReport += new ReportEventHandler(ReportListenerLib_OnReport);
                    m_reader.WaitMs = m_parms.ListenerTimeout;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to instantiate report reader", ex, true);
                return false;
            }
        }
         private bool SelectWriter(string writer)
        {
            try
            {
                m_writer = WriterFactory.GetWriter(writer);
                if (m_writer == null)
                {
                    LogInfoMessage(String.Format("Unable to instantiate report writer {0}", writer), true);
                    return false;
                }
                else
                {
                    m_writer.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                    m_writer.OnError += new LoggingEventHandler(Utilities_OnError);
                    m_writer.OnWriterStopped += new ServiceStoppedEventHandler(ReportListenerLib_OnWriterStopped);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to instantiate report writer", ex, true);
                return false;
            }
        }
      #endregion

        #region Start Listener
         private bool StartListener()
         {
             if (Login())
             {
                 if (m_bWriterStarted = StartWriter(m_parms.Writer))
                 {
                     return m_bReaderStarted = StartReader(m_parms.Reader);
                 }
             }

             return false;
         }

 
        private bool Login()
        {
            try
            {
                while (true)
                {
                    bool initilaized = false;
                    switch (m_parms.Reader)
                    {
                        case "RealTick":
                            initilaized = m_reader.Init(m_configDbConnection, m_parms.UserName);
                            break;
                        case "TWS":
                            initilaized = m_reader.Init();
                           break;

                    }

                    if (initilaized)
                    {
                        if (m_reader.Connect())
                            return true;
 
                       // if unable to start, retry as many times as specified in command parameters
                        m_reader.Dispose();
                        if (!Retry("Unable to log in"))
                            return false;
                        if (!SelectReader(m_parms.Reader))
                            return false;
                    }
                    else
                    {
                        LogInfoMessage("Unable to initialize report reader", true);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to log in", ex, true);
            }
            return false;
        }
 
        private bool StartWriter(string writer)
        {
            try
            {
                if (InitializeWriter(writer))
                {
                    if (m_writer.StartReportWriter())
                        return true;
                    else
                        LogInfoMessage("Unable to start report writer", true);
                }
                else
                {
                    LogInfoMessage("Unable to initialize report writer", true);
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to start report writer", ex, true);
            }
            return false;
        }

        private bool InitializeWriter(string writer)
        {
            try
            {
                switch (writer)
                {
                    case "Log":
                        if (m_writer.Init())
                            return true;
                        break;
                    case "Hugo":
                        if (m_writer.Init(m_hugoConnection2, m_parms.Reader, m_parms.HugoTimeout))
                           return true;
                        break;
                    default:
                        break;
                }
                LogInfoMessage(String.Format("Unable to initialzie report writer {0}", writer), true);
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to initialize report writer", ex, true);
            }
            return false;
        }

        private bool StartReader(string reader)
        {
            try
            {
                while (true)
                {
                    if (m_reader.StartReportReader())
                        return true;

                    // if unable to start, retry as many times as specified in command parameters
                    if (!Retry("Unable to start report reader"))
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to start report reader", ex, true);
            }
            return false;
        }

        private bool Retry(string msg)
        {

            if ((m_stopTime > DateTime.Now) && (m_parms.RetryCount-- != 0))
            {
                LogInfoMessage(msg + String.Format(" - retrying in {0} seconds", m_parms.RetryThrottle / 1000), true);

                m_bTaskStarted = !EndTask(m_parms.GetTaskName(), false);
                System.Threading.Thread.Sleep(m_parms.RetryThrottle);

                m_bTaskStarted = (0 == StartTask(m_parms.GetTaskName()));
                return true;
            }
            else
            {
                LogInfoMessage(msg + " - exceeded retry attempts", true);
                return false;
            }
        }
       #endregion

        #region Stop Listener
        private void StopListener()
        {
            StopReader();
            StopWriter();
        }
        private void StopReader()
        {
            if (m_bReaderStarted)
            {
                try
                {
                    m_bReaderStarted = false;
                    m_reader.StopReportReader(true);
                }
                catch (Exception ex)
                {
                    LogErrorMessage("Unable to stop reader", ex);
                }
             }
         }
        private void StopWriter()
        {
            if (m_bWriterStarted)
            {
                try
                {
                    m_bWriterStarted = false;
                    m_writer.StopReportWriter();
                }
                catch (Exception ex)
                {
                    LogErrorMessage("Unable to stop writer", ex);
                }
            }
        }
        #endregion

        #region Wait
        // returns true if terminated because we reached stopTime, false if terminated early
        private bool WaitForCompletion()
         {
            int tickTime = (int)(m_stopTime - DateTime.Now).TotalMilliseconds;
            if (tickTime <= 120000)
                tickTime = 120000;  // make sure stop time is at least 2 minutes from now

            LogInfoMessage(String.Format("Waiting for {0} ms", tickTime));
            if (WaitAny(tickTime, m_waitHandle))
            {
                LogInfoMessage("Job terminated early");
                return false;
            }
            else
            {
                LogInfoMessage("Job terminated on schedule");
                return true;
            }
         }
         private bool WaitAny(int millisecondsTimeout, params System.Threading.WaitHandle[] successConditionHandles)
         {
             int n;
             if (millisecondsTimeout == 0)
                 n = System.Threading.WaitHandle.WaitAny(successConditionHandles);
             else
                 n = System.Threading.WaitHandle.WaitAny(successConditionHandles, millisecondsTimeout);
             if (n == System.Threading.WaitHandle.WaitTimeout)
             {
                 return false;
             }
             else
             {
                 return true;
             }
         }
        #endregion

        #endregion
    }
}
