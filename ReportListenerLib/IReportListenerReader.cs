using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Common;

namespace ReportListenerLib
{
    public interface IReportListenerReader : IDisposable
    {
        bool Init(params object[] paramList);
        bool Connect();
        bool StartReportReader();
        bool StopReportReader(bool bReplay=false);

        event LoggingEventHandler OnError;
        event LoggingEventHandler OnInfo;
        event ReportEventHandler OnReport;
        event ServiceStoppedEventHandler OnReaderStopped;

        bool IsInitialized { get; }
        bool IsConnected { get; }
        bool IsReading { get; }

        bool HadError { get; }
        string LastErrorMessage { get; }
        int WaitMs { get; set; }
    }
}
