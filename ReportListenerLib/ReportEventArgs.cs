using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportListenerLib
{
    public delegate void ReportEventHandler(object sender, ReportEventArgs e);

    public class ReportEventArgs : EventArgs
    {
        public ReportEventArgs(string report)
        {
            Report = report;
        }
        public string Report { get; protected set; }
    }
}
