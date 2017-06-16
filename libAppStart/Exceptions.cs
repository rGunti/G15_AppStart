using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libAppStart
{
    public class EverythingIsFine : Exception
    {
        public EverythingIsFine(string message = "") : base(message) { }
    }

    public class FileAlreadyExistsException : Exception
    {
        public FileAlreadyExistsException(string message) : base(message) { }
    }

    public class AppListRecieved : Exception
    {
        private List<App> myAppList;

        public AppListRecieved(List<App> appList)
            : base("Data recieved!")
        {
            myAppList = appList;
        }

        public List<App> RecievedData { get { return myAppList; } }
    }
}
