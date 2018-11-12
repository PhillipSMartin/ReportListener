using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Utilities.CommandLine;
using System.IO;
using System.Diagnostics;

namespace ReportListenerConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineParameters parms = new CommandLineParameters();
            Listener listener = null;
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Gargoyle Strategic Investments\\ReportListenerConsole\\TraceListener.log";
            var trace = new TextWriterTraceListener(new StreamWriter(appDataPath, false));

            try
            {

                if (Gargoyle.Utilities.CommandLine.Utility.ParseCommandLineArguments(args, parms))
                {
 
                    listener = new Listener(parms);
                    if (listener.Run())
                    {
                        trace.WriteLine("ReportListener completed");
                     }
                    else
                    {
                        trace.WriteLine("ReportListener failed - see error log");
                    }
                }
                else
                {
                    // display usage message
                    string errorMessage = Gargoyle.Utilities.CommandLine.Utility.CommandLineArgumentsUsage(typeof(CommandLineParameters));

                    trace.WriteLine(errorMessage);
                 }
            }
            catch (Exception ex)
            {
                trace.WriteLine(ex.ToString());
            }
            finally
            {
                trace.Flush();
                if (listener != null)
                {
                    listener.Dispose();
                    listener = null;
                }
            }
        }

    }
}
