using System.Data.SqlClient;

namespace HugoReportWriterLib
{
    
    
    public partial class HugoDataSet {
    }
}
namespace HugoReportWriterLib.HugoDataSetTableAdapters
{


    public partial class QueriesTableAdapter
    {
        internal void SetAllCommandTimeouts(int timeOut)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal void SetAllConnections(SqlConnection sqlConnection)
        {
            foreach (System.Data.IDbCommand cmd in CommandCollection)
            {
                cmd.Connection = sqlConnection;
            }
        }
        public void LogCommand(string commandText)
        {
            HugoReportWriterUtilities.LogSqlCommand(CommandCollection, commandText);
        }

        internal int GetReturnCode(string commandText)
        {
            return HugoReportWriterUtilities.GetReturnCode(CommandCollection, commandText);
        }

    }
}
