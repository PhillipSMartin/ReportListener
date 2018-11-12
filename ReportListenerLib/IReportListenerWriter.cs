using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Common;

namespace ReportListenerLib
{
    public interface IReportListenerWriter : IDisposable
    {
        bool Init(params object[] paramList);
        bool StartReportWriter();
        bool StopReportWriter();

        event LoggingEventHandler OnError;
        event LoggingEventHandler OnInfo;
        event ServiceStoppedEventHandler OnWriterStopped;

        bool IsInitialized { get; }
        bool IsWriting { get; }

        bool HadError { get; }
        string LastErrorMessage { get; }

        // returns 0 on success, 1 if report is a duplicate, 4 on an error
        int TakeReport(string xml);
    }

}
