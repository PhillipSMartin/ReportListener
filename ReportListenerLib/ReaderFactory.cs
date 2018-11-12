using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportListenerLib
{
    public class ReaderFactory
    {
        private ReaderFactory() { }

        public static IReportListenerReader GetReader(string provider)
        {
            switch (provider)
            {
                case "RealTick":
                case "Realtick":
                    return new RealTickReportListener();

                case "TWS":
                case "IB":
                    return new TWSReportListener();

                default:
                    throw new ApplicationException(String.Format("Provider {0} is not supported by ReportListenerLib"));
            }
        }
    }
}
