using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportListenerLib
{
    public class WriterFactory
    {
        private WriterFactory() { }

        public static IReportListenerWriter GetWriter(string provider)
        {
            switch (provider)
            {
                case "Log":
                      return new LoggingReportWriter();

                case "Hugo":
                      return new HugoReportWriter();

                default:
                    throw new ApplicationException(String.Format("Provider {0} is not supported by ReportListenerLib", provider));
            }
        }
    }
}
