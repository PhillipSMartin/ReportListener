using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Utilities.CommandLine;

namespace ReportListenerConsole
{
    public class CommandLineParameters
    {
        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "r", Description = "Source of reports - currently only RealTick and TWS supported")]
        public string Reader = "RealTick";

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "w", Description = "Repository of reports - currently only Hugo and Log supported")]
        public string Writer = "Hugo";

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "pname", Description = "Name of program to specify to DBAccess")]
        public string ProgramName = "ReportListener";

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "tname", Description = "Template of task name for reporting completion (Username will replace {0}")]
        public string TaskName = "ReportListener - {0}";
 
        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "user", Description = "User - defaults to current login")]
        public string UserName = Environment.UserName;

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "ht", Description = "Time in milliseconds before Hugo events time out")]
        public int HugoTimeout = 10000;

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "lt", Description = "Time in milliseconds before ReportListener events time out")]
        public int ListenerTimeout = 30000;

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, Description = "Time app should automatically stop, expressed as a string hh:mm")]
        public string StopTime = "16:15";

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "retry", Description = "Number of times to retry login. -1 means infinite retries.")]
        public int RetryCount = -1;

        [CommandLineArgumentAttribute(CommandLineArgumentType.AtMostOnce, ShortName = "rt", Description = "Time in milliseconds between login retries")]
        public int RetryThrottle = 60000;

        public bool GetStopTime(out TimeSpan stopTime)
        {
            return TimeSpan.TryParse(StopTime, out stopTime);
        }

        public string GetTaskName()
        {
            try
            {
                return String.Format(TaskName, UserName);
            }
            catch (Exception)
            {
                return TaskName;
            }
        }
    }
}
