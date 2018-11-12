using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealTickLib;
using Gargoyle.Common;

namespace ReportListenerLib
{
    class RealTickReportListener : RealTickUtilities, IReportListenerReader
    {
        public RealTickReportListener()
        {
              base.OnReport += RealTickReportListener_OnReport;
      }

       private event ReportEventHandler m_reportEventHandler;
       void RealTickReportListener_OnReport(object sender, RealTickReportEventArgs e)
       {
           if (m_reportEventHandler != null)
               m_reportEventHandler(null, new ReportEventArgs(e.Report));
       }

        #region Overrides
        // there are two ways to initialize RealTickUtilites
        //  1) pass in a SqlConnection to the ConfigDb database - we will look up the proper credentials for the current user
        //  2) pass in three strings: username, password, and domainname
        public bool Init(params object[] paramList)
        {
            try
            {
                if (paramList.Length == 1)
                {
                    return base.Init((System.Data.SqlClient.SqlConnection)paramList[0]);
                }
                else if (paramList.Length == 2)
                {
                    return base.Init((System.Data.SqlClient.SqlConnection)paramList[0], (string)paramList[1]);
                }
                else if (paramList.Length == 3)
                {
                    return base.Init((string)paramList[0], (string)paramList[1], (string)paramList[2]);
                }
                else
                {
                    Info("Must pass either a connection to the ConfigDb database (with, optionally, a Windows username) or the Realtick username, password, and domainname of the realtick id you wish to use");
                }
            }
            catch (Exception ex)
            {
                Error("Unable to initialize RealTickUtilities", ex);
            }
            return false;
        }

        public new event ReportEventHandler OnReport
        {
            add { m_reportEventHandler += value; }
            remove { m_reportEventHandler -= value; }
        }

         #endregion
    }
}
