using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libConsoleReporter
{
    public static class ConsoleReporter
    {
        public enum ReportStatus { None, Info, Process, Warning, Error, DataReport };

        public static void Report(string message, ReportStatus status = ReportStatus.None, bool writeToFile = false)
        {
            string mstatus = String.Format("{0} {1}", GetStatusIcon(status), message);
            Console.WriteLine(mstatus);

            if (writeToFile)
                LogWriter.WriteToFile(mstatus);
        }

        static string GetStatusIcon(ReportStatus status)
        {
            switch (status)
            {
                case ReportStatus.Info:     return "[ i ]";
                case ReportStatus.Process:  return "[...]";
                case ReportStatus.Warning: return "[ ! ]";
                case ReportStatus.Error: return "[ X ]";
                case ReportStatus.DataReport: return "[>>>]";
                default:                    return "     ";
            }
        }
    }
}
