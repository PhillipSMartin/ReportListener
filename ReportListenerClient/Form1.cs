using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Gargoyle.Utils.DBAccess;
using LoggingUtilitiesLib;
using GargoyleTaskLib;
using ReportListenerLib;
using Gargoyle.Common;

namespace ReportListenerClient
{
    public partial class Form1 : Form
    {
        private ILog m_log = LogManager.GetLogger(typeof(Form1));
        private System.Data.SqlClient.SqlConnection m_hugoConnection1;
        private System.Data.SqlClient.SqlConnection m_hugoConnection2;
        private System.Data.SqlClient.SqlConnection m_hugoConnection3;
        private System.Data.SqlClient.SqlConnection m_configDbConnection;

        private HugoDataSet m_hugoDataSet = new HugoDataSet();
        private HugoDataSetTableAdapters.RealTickReports_RawDataTableAdapter m_realTickReportsRawData = new HugoDataSetTableAdapters.RealTickReports_RawDataTableAdapter();
        private HugoDataSetTableAdapters.RealTickReportsTableAdapter m_realTickReports = new HugoDataSetTableAdapters.RealTickReportsTableAdapter();
        private HugoDataSetTableAdapters.TWSReports_RawDataTableAdapter m_twsReportsRawData = new HugoDataSetTableAdapters.TWSReports_RawDataTableAdapter();
        private HugoDataSetTableAdapters.TWSReportsTableAdapter m_twsReports = new HugoDataSetTableAdapters.TWSReportsTableAdapter();

        private IReportListenerReader m_reader;
        private IReportListenerWriter m_writer;

        private bool m_connected;
        private bool m_readerStarted;
        private bool m_writerStarted;
        private bool m_taskStarted;
        private bool m_taskFailed;

        private string m_writerSelection = "";

        private string m_completionMessage;
        private string m_lastErrorMessage;

        private int m_reportsRead;
        private int m_reportsWritten;
        private int m_duplicatesIgnored;
        private int m_errors;

        System.Timers.Timer m_timer;

        public Form1()
        {
            InitializeComponent();
        }

        #region Housekeeping
        private void GetDatabaseConnections()
        {
            DBAccess dbAccess = DBAccess.GetDBAccessOfTheCurrentUser(Properties.Settings.Default.ProgramName);
            m_hugoConnection1 = dbAccess.GetConnection("Hugo");
            if (m_hugoConnection1 == null)
            {
                DisplayInfoMessage("Unable to connect to Hugo");
                Close();
                return;
            }
            m_hugoConnection2 = dbAccess.GetConnection("Hugo");
            if (m_hugoConnection2 == null)
            {
                DisplayInfoMessage("Unable to connect to Hugo");
                Close();
                return;
            }
            m_hugoConnection3 = dbAccess.GetConnection("Hugo");
            if (m_hugoConnection3 == null)
            {
                DisplayInfoMessage("Unable to connect to Hugo");
                Close();
                return;
            }
            m_realTickReports.Connection = m_hugoConnection3;
            m_realTickReports.SetAllCommandTimeouts(Properties.Settings.Default.TimeOut);
            m_realTickReportsRawData.Connection = m_hugoConnection3;
            m_realTickReportsRawData.SetAllCommandTimeouts(Properties.Settings.Default.TimeOut);
            m_twsReports.Connection = m_hugoConnection3;
            m_twsReports.SetAllCommandTimeouts(Properties.Settings.Default.TimeOut);
            m_twsReportsRawData.Connection = m_hugoConnection3;
            m_twsReportsRawData.SetAllCommandTimeouts(Properties.Settings.Default.TimeOut);

            m_configDbConnection = dbAccess.GetConnection("ConfigDB");
            if (m_configDbConnection == null)
            {
                DisplayInfoMessage("Unable to connect to ConfigDb");
                Close();
                return;
            }

         }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }

                Cleanup();
            }
            base.Dispose(disposing);
        }

        private void Cleanup()
        {
            if (m_timer != null)
            {
                m_timer.Dispose();
                m_timer = null;
            }
            if (m_reader != null)
            {
                m_reader.OnReport -= new ReportEventHandler(ReportListenerLib_OnReport);
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
            if (m_hugoConnection3 != null)
            {
                m_hugoConnection3.Dispose();
                m_hugoConnection3 = null;
            }

            if (m_configDbConnection != null)
            {
                m_configDbConnection.Dispose();
                m_configDbConnection = null;
            }

            if (m_realTickReportsRawData != null)
            {
                m_realTickReportsRawData.Dispose();
                m_realTickReportsRawData = null;
            }
            if (m_realTickReports != null)
            {
                m_realTickReports.Dispose();
                m_realTickReports = null;
            }
            if (m_twsReportsRawData != null)
            {
                m_twsReportsRawData.Dispose();
                m_twsReportsRawData = null;
            }
            if (m_twsReports != null)
            {
                m_twsReports.Dispose();
                m_twsReports = null;
            }
            if (m_hugoDataSet != null)
            {
                m_hugoDataSet.Dispose();
                m_hugoDataSet = null;
            }

            LoggingUtilities.OnInfo -= new LoggingEventHandler(Utilities_OnInfo);
            LoggingUtilities.OnError -= new LoggingEventHandler(Utilities_OnError);
            TaskUtilities.OnInfo -=  new LoggingEventHandler(Utilities_OnInfo);
            TaskUtilities.OnError -=  new LoggingEventHandler(Utilities_OnError);
        }
        #endregion

        #region Logging
        // events from LoggingUtilities and TaskUtilities
        private void Utilities_OnInfo(object sender, LoggingEventArgs eventArgs)
        {
            LogInfoMessage(eventArgs.Message, false);
        }

        private void Utilities_OnError(object sender, LoggingEventArgs eventArgs)
        {
            LogErrorMessage(eventArgs.Message, eventArgs.Exception);
        }

        // helper methods to write to log
        private void LogInfoMessage(string msg, bool updateStatus = true)
        {
            if (m_log != null)
            {
                lock (m_log)
                {
                    m_log.Info(msg);
                }
            }

            if (updateStatus)
            {
                SetStatusMsg(msg);
            }

            Action a = delegate
            {
                listBoxLog.Items.Add(msg);
            };
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.Invoke();
            }

        }
        private void LogErrorMessage(string msg, Exception e, bool updateStatus = true)
        {
            if (m_log != null)
            {
                lock (m_log)
                {
                    m_log.Error(msg, e);
                }
            }

            if (updateStatus)
            {
                SetStatusMsg("Error - see log");
            }
        }

        // helper methods to display dialog box
        private void DisplayInfoMessage(string msg)
        {
            LogInfoMessage(msg, false);

            Action a = delegate
            {
                MessageBox.Show(msg);
            };
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.Invoke();
            }
        }
        private void DisplayErrorMessage(string msg, Exception e)
        {
            LogErrorMessage(msg, e, false);

            Action a = delegate
            {
                if (e == null)
                    MessageBox.Show(msg);
                else
                    MessageBox.Show(msg + "->" + e.Message);
            };
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.Invoke();
            }
        }
        #endregion

        #region Wired Event Handlers
        private void ReportListenerLib_OnReport(object sender, ReportEventArgs e)
        {
            System.Threading.Interlocked.Increment(ref m_reportsRead);
            int returnCode = m_writer.TakeReport(e.Report);
            if (returnCode == 0)
                System.Threading.Interlocked.Increment(ref m_reportsWritten);
            else if (returnCode == 1)
                System.Threading.Interlocked.Increment(ref m_duplicatesIgnored);
            else
                System.Threading.Interlocked.Increment(ref m_errors);
        }
        private void ReportListenerLib_OnReaderStopped(object sender, ServiceStoppedEventArgs e)
        {
            LogInfoMessage(e.Message, true);

            m_readerStarted = false;
            if (m_reader.HadError)
            {
                m_taskFailed = true;
                m_lastErrorMessage = m_reader.LastErrorMessage;
            }
        }
        private void ReportListenerLib_OnWriterStopped(object sender, ServiceStoppedEventArgs e)
        {
            LogInfoMessage(e.Message, true);

            m_writerStarted = false;
            if (m_writer.HadError)
            {
                m_taskFailed = true;
                m_lastErrorMessage = m_writer.LastErrorMessage;
            }

            StopReader();
        }
        #endregion

        #region Form Event Handler
        private int m_bindingCount = 0;
        private void OnTimerFired(object sender, EventArgs e)
        {
            if (1 >= System.Threading.Interlocked.Increment(ref m_bindingCount))
            {
                Action a = (Action)delegate
                {
                    BindSource();
                    System.Threading.Interlocked.Decrement(ref m_bindingCount);
                };

                if (InvokeRequired)
                {
                    BeginInvoke(a);
                }
                else
                {
                    a.Invoke();
                }
            }
            else
            {
                System.Threading.Interlocked.Decrement(ref m_bindingCount);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopReader();
            StopWriter();

            Cleanup();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // initialize logging
                log4net.Config.XmlConfigurator.Configure();
                LoggingUtilities.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                LoggingUtilities.OnError += new LoggingEventHandler(Utilities_OnError);
                TaskUtilities.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                TaskUtilities.OnError += new LoggingEventHandler(Utilities_OnError);

                GetDatabaseConnections();
                SetHeader();
                SelectReader(Properties.Settings.Default.ReaderSelection);

                m_timer = new System.Timers.Timer();
                m_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerFired);
                m_timer.Interval = Properties.Settings.Default.AutoRefreshInterval;
                m_timer.Enabled = checkBoxAutoRefresh.Checked;

                BindSource();
                EnableControls();
            }
            catch (Exception ex)
            {
                DisplayErrorMessage("Error initializing ReportListener", ex);
                Close();
            }
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            Enabled = false;
 
            if (ConnectToReader())
                SetStatusMsg("Successfully logged in to reader");
            else
                SetStatusMsg("Unable to log in to reader" );
            EnableControls();
            Activate();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Enabled = false;
            if (StartWriter())
            {
                StartReader();
            }

            if (m_readerStarted)
            {
                string taskMsg = "";
                if (!String.IsNullOrEmpty(TaskName))
                {
                    taskMsg = " - " + TaskName + (StartTask() ? " started" : " could not be started");
                }
                SetStatusMsg("Successfully started listener" + taskMsg);
            }
            else
                SetStatusMsg("Unable to start listener");
            EnableControls();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Enabled = false;
            StopReader();
            StopWriter();

            if (!m_readerStarted)
                SetStatusMsg("Successfully stopped listener");
            else
                SetStatusMsg("Unable to stop listener");
            EnableControls();
        }

        private void radioButtonLogWriter_Click(object sender, EventArgs e)
        {
            Enabled = false;
            SelectWriter();
            EnableControls();
        }

        private void radioButtonHugoWriter_Click(object sender, EventArgs e)
        {
            Enabled = false;
            SelectWriter();
            EnableControls();
        }

        private void radioButtonRealTick_Click(object sender, EventArgs e)
        {
            Enabled = false;
            SelectReader("RealTick");
            EnableControls();
        }

        private void radioButtonTWS_Click(object sender, EventArgs e)
        {
            Enabled = false;
            SelectReader("TWS");
            EnableControls();

        }

        private void checkBoxAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            m_timer.Enabled = checkBoxAutoRefresh.Checked;
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            BindSource();
        }

        private void radioButtonDisplay_Click(object sender, EventArgs e)
        {
            tabPageHugoData.Text = DisplaySelection;
            BindSource();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            BindSource();
        }

        private void checkBoxConsolidate_Click(object sender, EventArgs e)
        {
            BindSource();
        }
        #endregion

        #region Private Properties
        private string TaskName
        {
            get { return String.Format(Properties.Settings.Default.TaskName, Environment.UserName); }
        }
        private DateTime ImportDate { get { return dateTimePicker1.Value; } }
        private string DisplaySelection
        {
            get
            {
                if (radioButtonReports.Checked)
                    return "Reports";
                else
                    return "Raw Data";
            }
        }
        private bool Consolidate { get { return checkBoxConsolidate.Checked; } }
        private string Reader
        {
            get
            {
                if (radioButtonRealTick.Checked)
                    return "RealTick";
                else
                    return "TWS";
            }
        }
        #endregion

        #region Private Methods
        private void SetHeader()
        {
            Text = String.Format("{0} {1}",
                  System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                  System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            if (m_hugoConnection1 != null)
                Text += " - " + m_hugoConnection1.DataSource;
        }

        private void SetStatusMsg(string msg)
        {
            Action a = (Action)delegate { labelStatus.Text = msg; };

            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.Invoke();
            }
        }
   
        private void EnableControls()
        {
            Action a = delegate
            {
                Enabled = true;
                buttonLogIn.Enabled = !m_connected;
                buttonStart.Enabled = m_connected && (!m_readerStarted) && (!m_writerStarted);
                buttonStop.Enabled = m_readerStarted || m_writerStarted;

                radioButtonHugoWriter.Enabled = radioButtonLogWriter.Enabled = !m_writerStarted;
            };
            if (InvokeRequired)
            {
                BeginInvoke(a);
            }
            else
            {
                a.Invoke();
            }
        }

        private void BindSource()
        {
            switch (DisplaySelection)
            {
                case "Reports":
                    dataGridView1.DataSource = GetHugoReports(ImportDate);
                    SetStatusMsg(String.Format("Updated {0}", DateTime.Now.ToLongTimeString()));
                    break;
                case "Raw Data":
                    dataGridView1.DataSource = GetHugoReportsRawData(ImportDate);
                    SetStatusMsg(String.Format("Updated {0}", DateTime.Now.ToLongTimeString()));
                   break;
                default:
                    dataGridView1.DataSource = null;
                    break;
            }

            labelReceived.Text = m_reportsRead.ToString();
            labelSaved.Text = m_reportsWritten.ToString();
            labelErrors.Text = m_errors.ToString();
        }

        private DataTable GetHugoReports(DateTime? importDate)
        {
            try
            {
                switch (Reader)
                {
                    case "RealTick":
                        return m_realTickReports.GetData(importDate, Consolidate);
                    case "TWS":
                        return m_twsReports.GetData(importDate, Consolidate);
                    default:
                        LogInfoMessage("Invalid reader");
                        return null;
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to get Reports", ex);
                return null;
            }
        }
        private DataTable GetHugoReportsRawData(DateTime? importDate)
        {
            try
            {
                switch (Reader)
                {
                    case "RealTick":
                        return m_realTickReportsRawData.GetData(importDate);
                    case "TWS":
                        return m_twsReportsRawData.GetData(importDate);
                    default:
                        LogInfoMessage("Invalid reader");
                        return null;
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage("Unable to get raw data", ex);
                return null;
            }
        }
        #endregion

        #region Start/Stop Methods
        private bool StartTask()
        {
            if (!m_taskStarted)
            {
                try
                {
                    using (TaskUtilities taskUtilities = new TaskUtilities(m_hugoConnection1, Properties.Settings.Default.TimeOut))
                    {
                        m_taskStarted = (0 == taskUtilities.TaskStarted(TaskName, null));
                    }
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to start task", ex);
                }
            }
            return m_taskStarted;
        }
        private bool EndTask(bool succeeded)
        {
            if (m_taskStarted)
            {
                try
                {
                    using (TaskUtilities taskUtilities = new TaskUtilities(m_hugoConnection1, Properties.Settings.Default.TimeOut))
                    {
                        if (succeeded)
                            m_taskStarted = (0 != taskUtilities.TaskCompleted(TaskName, m_completionMessage));
                        else
                            m_taskStarted = (0 != taskUtilities.TaskFailed(TaskName, m_lastErrorMessage));
                    }
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to stop task", ex);
                }
            }
            return !m_taskStarted;
        }
        private bool SelectReader(string reader)
        {
            try
            {
                if (m_reader != null)
                {
                    m_reader.OnInfo -= new LoggingEventHandler(Utilities_OnInfo);
                    m_reader.OnError -= new LoggingEventHandler(Utilities_OnError);
                    m_reader.OnReaderStopped -= new ServiceStoppedEventHandler(ReportListenerLib_OnReaderStopped);
                    m_reader.OnReport -= new ReportEventHandler(ReportListenerLib_OnReport);
                }

                m_reader = ReaderFactory.GetReader(reader);
                if (m_reader != null)
                {
                    m_reader.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                    m_reader.OnError += new LoggingEventHandler(Utilities_OnError);
                    m_reader.OnReaderStopped += new ServiceStoppedEventHandler(ReportListenerLib_OnReaderStopped);
                    m_reader.OnReport += new ReportEventHandler(ReportListenerLib_OnReport);
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage("Unable to initialize reader", ex);
            }
            return (m_reader != null);
        }
        private bool ConnectToReader()
        {
            if (m_reader != null)
            {
                if (!m_connected)
                {
                    try
                    {
                        if (Reader == "RealTick")
                        {
                            if (m_reader.Init(m_configDbConnection))
                            {
                                m_connected = m_reader.Connect();
                            }
                        }
                        else
                        {
                            if (m_reader.Init())
                            {
                                m_connected = m_reader.Connect();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayErrorMessage("Unable to log in to reader", ex);
                    }
                }
             }
            return m_connected;
        }

        private bool StartReader()
        {
            if (!m_readerStarted)
            {
                try
                {
                    if (m_reader != null)
                    {
                        m_reportsRead = 0;
                        m_reportsWritten = 0;
                        m_duplicatesIgnored = 0;
                        m_errors = 0;
                        if (m_reader.IsConnected)
                            m_readerStarted = m_reader.StartReportReader();
                    }
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to start reader", ex);
                }
            }
            return m_readerStarted;
        }
        private bool StopReader()
        {
            if (m_readerStarted)
            {
                try
                {
                    m_readerStarted = !m_reader.StopReportReader(true);
                    m_connected = m_reader.IsConnected;

                    if (!m_readerStarted)
                    {
                        m_completionMessage = String.Format("Wrote {0} out of {1} reports, {2} duplicates ignored, {3} errors", m_reportsWritten, m_reportsRead, m_duplicatesIgnored, m_errors);
                        if (!String.IsNullOrEmpty(TaskName))
                        {
                            EndTask(!m_taskFailed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to stop reader", ex);
                }
            }
            return !m_readerStarted;
        }

        private bool SelectWriter()
        {
            try
            {
                string newWriterSelection;
                if (radioButtonHugoWriter.Checked)
                    newWriterSelection = "Hugo";
                else
                    newWriterSelection = "Log";

                if (newWriterSelection != m_writerSelection)
                {
                    if (m_writer != null)
                    {
                        m_writer.OnInfo -= new LoggingEventHandler(Utilities_OnInfo);
                        m_writer.OnError -= new LoggingEventHandler(Utilities_OnError);
                        m_writer.OnWriterStopped -= new ServiceStoppedEventHandler(ReportListenerLib_OnWriterStopped);
                    }

                    m_writer = WriterFactory.GetWriter(newWriterSelection);
                    m_writerSelection = newWriterSelection;

                    if (m_writer != null)
                    {
                        m_writer.OnInfo += new LoggingEventHandler(Utilities_OnInfo);
                        m_writer.OnError += new LoggingEventHandler(Utilities_OnError);
                        m_writer.OnWriterStopped += new ServiceStoppedEventHandler(ReportListenerLib_OnWriterStopped);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage("Unable to initialize writer", ex);
            }
            return (m_writer != null);
        }
         private bool InitializeWriter()
        {
            if (!m_writer.IsInitialized)
            {
                try
                {
                    switch (m_writerSelection)
                    {
                        case "Log":
                            m_writer.Init();
                            break;
                        case "Hugo":
                            m_writer.Init(m_hugoConnection2, Reader, Properties.Settings.Default.TimeOut);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to initialize writer", ex);
                }
            }
            return m_writer.IsInitialized;
        }
         private bool StartWriter()
         {
             if (!m_writerStarted)
             {
                 try
                 {
                     if (m_writer == null)
                         SelectWriter();
                     if (m_writer != null)
                     {
                         if (InitializeWriter())
                             m_writerStarted = m_writer.StartReportWriter();
                     }
                     else
                     {
                         DisplayInfoMessage("No writer selected");
                     }
                 }
                 catch (Exception ex)
                 {
                     DisplayErrorMessage("Unable to start writer", ex);
                 }
             }
             return m_writerStarted;
         }
         private bool StopWriter()
        {
            if (m_writerStarted)
            {
                try
                {
                    m_writerStarted = !m_writer.StopReportWriter();
                }
                catch (Exception ex)
                {
                    DisplayErrorMessage("Unable to stop writer", ex);
                }
            }
            return !m_writerStarted;
        }
        #endregion



    }
}
