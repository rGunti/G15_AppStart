using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace libAppStart
{
    public class App
    {
        private string myTitle;
        private string myPath;

        public App(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            myPath = path;
            myTitle = App.GetApplicationName(path);
        }

        public App(string path, string title)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            if (String.IsNullOrWhiteSpace(title))
                myTitle = App.GetApplicationName(path);
            else
                myTitle = title;

            myPath = path;
        }

        public string Name { get { return myTitle; } set { myTitle = value; } }

        public string Path { get { return myPath; } set { myPath = value; } }

        public static string GetApplicationName(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            string appname = FileVersionInfo.GetVersionInfo(path).ProductName;

            if (String.IsNullOrWhiteSpace(appname))
                return System.IO.Path.GetFileNameWithoutExtension(path);
            else
                return appname;
        }

        public void StartApp()
        {
            if (!File.Exists(myPath))
                throw new FileNotFoundException();

            Process.Start(myPath);
        }
    }
}
