using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace G15_AppStart
{
    static class Constants
    {
        public static string APP_NAME = "g15 AppStart";
        public static string APP_VERSION = "0.1 beta";
        public static string APP_COPYRIGHT = "© 2014, rGunti";
        public static readonly string APP_INFO = String.Format("{0}\nVer. {1}\n{2}",
            APP_NAME, APP_VERSION, APP_COPYRIGHT);

        public static readonly string DB_FILEPATH = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "apps.db");
    }
}
