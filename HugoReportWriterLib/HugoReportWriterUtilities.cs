using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using LoggingUtilitiesLib;
using Gargoyle.Common;
    
namespace HugoReportWriterLib
{
    public class HugoReportWriterUtilities
    {
        private string m_reader;

        public HugoReportWriterUtilities() { }

        private HugoDataSetTableAdapters.QueriesTableAdapter m_queriesTableAdapter = new HugoDataSetTableAdapters.QueriesTableAdapter();

        #region Event handlers
        private static event LoggingEventHandler s_infoEventHandler;
        private static event LoggingEventHandler s_errorEventHandler;
        private event ServiceStoppedEventHandler m_writerStoppedEventHandler;

        // event fired when an exception occurs
        public static event LoggingEventHandler OnError
        {
            add { s_errorEventHandler += value; }
            remove { s_errorEventHandler -= value; }
        }
        // event fired for logging
        public static event LoggingEventHandler OnInfo
        {
            add { s_infoEventHandler += value; }
            remove { s_infoEventHandler -= value; }
        }
        // event fired when writer stops
        public event ServiceStoppedEventHandler OnWriterStopped
        {
            add { m_writerStoppedEventHandler += value; }
            remove { m_writerStoppedEventHandler -= value; }
        }
        #endregion

        #region Public Properties
        public bool IsInitialized { get; private set; }
        public bool IsWriting { get; private set; }
        public static bool HadError { get; private set; }
        public static string LastErrorMessage { get; private set; }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public bool Init(SqlConnection sqlConnection, string reader, int timeOut=10000)
        {
            if (!IsInitialized)
            {
                try
                {
                    if (sqlConnection == null)
                        throw new ArgumentNullException("sqlConnection");

                    m_reader = reader.ToUpper();
                    switch (m_reader)
                    {
                        case "REALTICK":
                        case "TWS":
                            break;
                        default:
                            throw new Exception("Reader must be either RealTick or TWS");
                    }

                    m_queriesTableAdapter.SetAllConnections(sqlConnection);
                    m_queriesTableAdapter.SetAllCommandTimeouts(timeOut);

                    Info("HugoReportWriterUtilities SQL connection = " + sqlConnection.ConnectionString);

                    IsInitialized = true;
                    Info("HugoReportWriterUtilities initialized");
                }
                catch (Exception ex)
                {
                    Error("Unable to initialize HugoReportWriterUtilities", ex);
                }
            }
            return IsInitialized;
         }
        public bool StartReportWriter()
        {
            if (IsInitialized)
                IsWriting = true;
            return IsWriting;
        }
        public bool StopReportWriter()
        {
            if (IsWriting)
            {
                try
                {
                    IsWriting = false;
                    if (m_writerStoppedEventHandler != null)
                        m_writerStoppedEventHandler(null, new ServiceStoppedEventArgs("HugoReportWriter", "ReportWriter stopped"));
                    else
                     Info("ReportWriter stopped");
               }
                catch (Exception ex)
                {
                    Error("Error in OnWriterStopped handler", ex);
                }
             }
             return true;
        }

        // returns 0 on success, 1 if report is a duplicate, 4 or higher on an error
        public int TakeReport(string xml)
        {
            int returnCode = 4;
            int? recordId = null;

            if (IsWriting)
            {
                ClearErrorState();

                try
                {
                    switch (m_reader)
                    {
                        case "REALTICK":
                            m_queriesTableAdapter.InsertRealTickReport(xml, ref recordId, null, null, null);
                            returnCode = m_queriesTableAdapter.GetReturnCode("dbo.p_insert_RealTickReport");
                            break;
                        case "TWS":
                            m_queriesTableAdapter.InsertTWSReport(xml, ref recordId, null, null);
                            returnCode = m_queriesTableAdapter.GetReturnCode("TWS.p_insert_Report");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Error("Error taking report", ex);
                }
                finally
                {
                    switch (m_reader)
                    {
                        case "REALTICK":
                            m_queriesTableAdapter.LogCommand("dbo.p_insert_RealTickReport");
                            break;
                         case "TWS":
                            m_queriesTableAdapter.LogCommand("TWS.p_insert_Report");
                            break;
                    }
                }
            }

            if (returnCode > 1)
            {
                Info("FAILED TRANSACTION:" + xml);
            }
            return returnCode;
        }
        #endregion

        #region Private Methods
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopReportWriter();
                if (m_queriesTableAdapter != null)
                {
                    m_queriesTableAdapter.Dispose();
                    m_queriesTableAdapter = null;
                }
               Info("HugoReportWriterUtilities disposed");
            }
        }
        internal static void LogSqlCommand(IDbCommand[] commandCollection, string commandText)
        {
            try
            {
                LoggingUtilities.LogSqlCommand("SqlLog", commandCollection, commandText);
            }
            catch (LoggingUtilitiesCommandNotFoundException e)
            {
                Info("Logging error: " + e.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static int GetReturnCode(IDbCommand[] commandCollection, string commandText)
        {
            try
            {
                return LoggingUtilities.GetReturnCode(commandCollection, commandText);
            }
            catch (LoggingUtilitiesCommandNotFoundException e)
            {
                Info("Logging error: " + e.Message);
                return 8;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Logging
        protected static  void Info(string msg)
        {
            try
            {
                if (s_infoEventHandler != null)
                {
                    s_infoEventHandler(null, new LoggingEventArgs("HugoReportWriterLib", msg));
                }
            }
            catch
            {
            }
        }

        protected static void Error(string msg, Exception e)
        {
            try
            {
                HadError = true;
                LastErrorMessage = e.Message;
                if (s_errorEventHandler != null)
                    s_errorEventHandler(null, new LoggingEventArgs("HugoReportWriterLib", msg, e));
            }
            catch
            {
            }
        }

        private void ClearErrorState()
        {
            HadError = false;
            LastErrorMessage = null;
        }
        #endregion
        #endregion
    }
}
