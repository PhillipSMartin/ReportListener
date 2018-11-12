namespace ReportListenerClient {
    
    
    public partial class HugoDataSet {
    }
}

namespace ReportListenerClient.HugoDataSetTableAdapters {


    public partial class RealTickReportsTableAdapter
    {
        public void SetAllCommandTimeouts(int timeOut)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
    }
    public partial class RealTickReports_RawDataTableAdapter
    {
        public void SetAllCommandTimeouts(int timeOut)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
    }
    public partial class TWSReportsTableAdapter
    {
        public void SetAllCommandTimeouts(int timeOut)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
    }
    public partial class TWSReports_RawDataTableAdapter
    {
        public void SetAllCommandTimeouts(int timeOut)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
    }
}
