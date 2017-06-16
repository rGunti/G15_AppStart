using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace libConsoleReporter
{
    static class LogWriter
    {
        public static void WriteToFile(string message)
        {
            try
            {
                FileStream fs = new FileStream("g15appstart.log", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(String.Format("{0:yyyy-MM-dd hh:mm:ss.ffff} : {1}", DateTime.Now, message));
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
