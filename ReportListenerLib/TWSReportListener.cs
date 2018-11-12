using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWSLib;

namespace ReportListenerLib
{
    class TWSReportListener : TWSUtilities, IReportListenerReader
    {
        #region Event Handlers
        private event ReportEventHandler m_reportEventHandler;
        public new event ReportEventHandler OnReport
        {
            add { m_reportEventHandler += value; }
            remove { m_reportEventHandler -= value; }
        }
        #endregion

        public TWSReportListener()
        {
            base.OnReport += TWSReportListener_OnReport;
        }
        #region Overrides
        // Init accepts an optional IPAddress and port 
        public bool Init(params object[] paramList)
        {
            try
            {
                if (paramList.Length == 0)
                {
                    return base.Init(TWSUtilities.REPORT_READER);
                }
                else if (paramList.Length == 1)
                {
                     return base.Init(TWSUtilities.REPORT_READER, (string)paramList[0]);
                }
                else if (paramList.Length == 2)
                {
                      return base.Init(TWSUtilities.REPORT_READER, (string)paramList[0], (int)paramList[1]);
                }
                else
                {
                    Info("Init accepts an optional IPAddress (a string), followed by an optional port (an int)");
                }
            }
            catch (Exception ex)
            {
                Error("Unable to initialize TWSUtilities", ex);
            }
            return false;
        }
        #endregion

        private void TWSReportListener_OnReport(object sender, TWSReportEventArgs e)
        {
            if (m_reportEventHandler != null)
                m_reportEventHandler(null, new ReportEventArgs(e.Report));
        }
    }
}
