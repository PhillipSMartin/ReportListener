using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugoReportWriterLib;
using Gargoyle.Common;

namespace ReportListenerLib
{
    class HugoReportWriter : HugoReportWriterUtilities, IReportListenerWriter
    {
        public HugoReportWriter()
        {
        }

        #region Events
        public new event LoggingEventHandler OnError
        {
            add { HugoReportWriterUtilities.OnError += value; }
            remove { HugoReportWriterUtilities.OnError -= value; }
        }
        public new event LoggingEventHandler OnInfo
        {
            add { HugoReportWriterUtilities.OnInfo += value; }
            remove { HugoReportWriterUtilities.OnInfo -= value; }
        }
        #endregion

        #region Overrides
        public bool Init(params object[] paramList)
        {
            try
            {
                if (paramList.Length == 2)
                {
                    return base.Init((System.Data.SqlClient.SqlConnection)paramList[0], (string)paramList[1]);
                }
                else if (paramList.Length == 3)
                {
                    return base.Init((System.Data.SqlClient.SqlConnection)paramList[0], (string)paramList[1], (int)paramList[2]);
                }
                else
                {
                    Info("Must pass a connection to the Hugo database, a reader, and (optionally) a timeout value in milleseconds");
                }
            }
            catch (Exception ex)
            {
                Error("Unable to initialize HugoReportWriterUtilities", ex);
            }
            return false;
        }

        public new bool HadError { get { return HugoReportWriterUtilities.HadError; } }
        public new string LastErrorMessage { get { return HugoReportWriterUtilities.LastErrorMessage; } }
        #endregion
    }
}
