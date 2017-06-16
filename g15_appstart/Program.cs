using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;

using GammaJul.LgLcd;
using libAppStart;
using libConsoleReporter;

namespace G15_AppStart
{
    class Program
    {
        private static readonly Random _random = new Random();
        private static readonly AutoResetEvent _waitAre = new AutoResetEvent(false);
        private static volatile bool _monoArrived;
        private static volatile bool _mustExit;

        private static LcdDevice _device;
        private static LcdApplet _applet;

        private static bool _logEnabled = false;

        private static bool _configButtonEnabled;

        private static Database _db;
        private static List<App> appList;

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                _logEnabled = (args[0] == "-debug");
                ConsoleReporter.Report(String.Format("Log {0}", (_logEnabled) ? "enabled" : "disabled"), ConsoleReporter.ReportStatus.Info, _logEnabled);
            }

            Console.WriteLine("{0}\nVersion {1}\n{2}\n", Constants.APP_NAME.ToUpper(), Constants.APP_VERSION, Constants.APP_COPYRIGHT);

            InitDB();
            InitApp();
            InitSettings();

            _device = null;
            _waitAre.WaitOne();
            ConsoleReporter.Report("WaitOne done", ConsoleReporter.ReportStatus.None, _logEnabled);

            do
            {
                if (_monoArrived)
                {
                    ConsoleReporter.Report("Mono Device arrived", ConsoleReporter.ReportStatus.Info, _logEnabled);
                    if (_device == null)
                    {
                        _device = (LcdDeviceMonochrome)_applet.OpenDeviceByType(LcdDeviceType.Monochrome);
                        _device.SoftButtonsChanged += _device_SoftButtonsChanged;
                        InitGraphics(_device);
                    }
                    else _device.ReOpen();

                    _monoArrived = false;
                }

                if (_applet.IsEnabled && _device != null && !_device.IsDisposed)
                    _device.DoUpdateAndDraw();
            } while (!_mustExit);

            if (_applet.IsEnabled && _device != null && !_device.IsDisposed)
                _device.DoUpdateAndDraw();

            ConsoleReporter.Report("Applet done! Quitting...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            Thread.Sleep(10);
        }

        #region Init
        static void InitDB()
        {
            ConsoleReporter.Report("Checking availability of database...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            ConsoleReporter.Report(String.Format("File exists: {0}", File.Exists("apps.db")), ConsoleReporter.ReportStatus.DataReport, _logEnabled);

            // Init
            ConsoleReporter.Report("Initializing database...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            _db = new Database(Constants.DB_FILEPATH);
            try
            {
                _db.InitializeDatabase();
            }
            catch (FileAlreadyExistsException ex)
            {
                ConsoleReporter.Report("File does already exists, continue initialisation...", ConsoleReporter.ReportStatus.Error, _logEnabled);
            }
            catch (Exception ex)
            {
                ConsoleReporter.Report("An error occured!", ConsoleReporter.ReportStatus.Error, _logEnabled);
                ConsoleReporter.Report(ex.Message);
            }
            ConsoleReporter.Report(String.Format("Init of '{0}' completed", _db.FileName), ConsoleReporter.ReportStatus.Info, _logEnabled);
            ConsoleReporter.Report("Loading apps...", ConsoleReporter.ReportStatus.Process, _logEnabled);

            // Load Data
            Exception getData = _db.GetAllApplications(Database.OrderBy.AppName);

            if (typeof(AppListRecieved) != getData.GetType())
            {
                ConsoleReporter.Report(getData.Message, ConsoleReporter.ReportStatus.Error, _logEnabled);
                return;
            }
            else
            {
                ConsoleReporter.Report("Data recieved", ConsoleReporter.ReportStatus.Info, _logEnabled);
            }

            appList = ((AppListRecieved)getData).RecievedData;
            ConsoleReporter.Report(String.Format("{0} app(s) loaded", appList.Count), ConsoleReporter.ReportStatus.Info, _logEnabled);
        }

        static void InitApp()
        {
            ConsoleReporter.Report("Initializing applet...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            _applet = new LcdApplet(Constants.APP_NAME, LcdAppletCapabilities.Monochrome);

            _applet.Configure += _applet_Configure;
            _applet.DeviceArrival += _applet_DeviceArrival;
            _applet.DeviceRemoval += _applet_DeviceRemoval;
            _applet.IsEnabledChanged += _applet_IsEnabledChanged;

            ConsoleReporter.Report("Applet initialized", ConsoleReporter.ReportStatus.Info, _logEnabled);
            ConsoleReporter.Report("Connecting applet...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            _applet.Connect();
            ConsoleReporter.Report("Connection completed", ConsoleReporter.ReportStatus.Info, _logEnabled);
        }

        static void InitSettings()
        {
            ConsoleReporter.Report("Loading settings...", ConsoleReporter.ReportStatus.Process, _logEnabled);

            // Get Settings
            Preference PREF_4th_openConfigEnabled = _db.GetPreferenceByKey("4th_OpenConfigEnabled");
            if (PREF_4th_openConfigEnabled != null)
            {
                if (PREF_4th_openConfigEnabled.Type == typeof(bool))
                {
                    _configButtonEnabled = (bool)PREF_4th_openConfigEnabled.Value;
                }
                else
                {
                    _configButtonEnabled = false;
                }
            }
            else
            {
                _configButtonEnabled = false;
            }
            ConsoleReporter.Report(String.Format("4th_OpenConfigEnabled: (BOOL) {0}", _configButtonEnabled), ConsoleReporter.ReportStatus.Error, _logEnabled);
            ConsoleReporter.Report("Loading settings completed", ConsoleReporter.ReportStatus.Info, _logEnabled);
        }
        #endregion Init

        #region Events
        static void _applet_Configure(object sender, EventArgs e)
        {
            bool quitApplet = false;

            ConsoleReporter.Report("Enter Config Mode...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            _currentPageIndex = 2;
            _inConfigMode = true;

            AppStart_DBClient.Form1 configForm = new AppStart_DBClient.Form1(Constants.DB_FILEPATH);
            quitApplet = (configForm.ShowDialog() == System.Windows.Forms.DialogResult.Abort);

            ConsoleReporter.Report("Config completed!", ConsoleReporter.ReportStatus.Info, _logEnabled);

            if (quitApplet)
            {
                _mustExit = true;
                return;
            }

            InitDB();
            InitSettings();
            InitGraphics(_device);
            _currentPageIndex = 0;
            _inConfigMode = false;
        }

        static void _applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            switch (e.DeviceType)
            {
                case LcdDeviceType.Monochrome:
                    _monoArrived = true;
                    break;
            }
            _waitAre.Set();
        }

        static void _applet_DeviceRemoval(object sender, LcdDeviceTypeEventArgs e)
        {
            ConsoleReporter.Report("Device disconnected!", ConsoleReporter.ReportStatus.Warning, _logEnabled);
        }

        static void _applet_IsEnabledChanged(object sender, EventArgs e)
        {
            ConsoleReporter.Report("Applet got " + ((_applet.IsEnabled) ? "enabled" : "disabled"), ConsoleReporter.ReportStatus.Warning, _logEnabled);
        }

        static void _device_SoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            if (_inConfigMode) return;

            if ((e.SoftButtons & LcdSoftButtons.Button0) == LcdSoftButtons.Button0)
            {
                ConsoleReporter.Report("BUTTON_0 pressed", ConsoleReporter.ReportStatus.Error, _logEnabled);

                if (_exitQuestion)
                {
                    _mustExit = true;
                    return;
                }

                sview.MoveSelectorUp();
            }
            if ((e.SoftButtons & LcdSoftButtons.Button1) == LcdSoftButtons.Button1)
            {
                ConsoleReporter.Report("BUTTON_1 pressed", ConsoleReporter.ReportStatus.Error, _logEnabled);

                if (_exitQuestion) return;

                sview.MoveSelectorDown();
            }
            if ((e.SoftButtons & LcdSoftButtons.Button2) == LcdSoftButtons.Button2)
            {
                ConsoleReporter.Report("BUTTON_2 pressed", ConsoleReporter.ReportStatus.Error, _logEnabled);

                if (_exitQuestion)
                    return;

                sview.ExecuteSelected();
            }
            if ((e.SoftButtons & LcdSoftButtons.Button3) == LcdSoftButtons.Button3)
            {
                ConsoleReporter.Report("BUTTON_3 pressed", ConsoleReporter.ReportStatus.Error, _logEnabled);

                if (sview.ConfigButtonsEnabled)
                {
                    ConsoleReporter.Report("Config pressed", ConsoleReporter.ReportStatus.None, _logEnabled);

                    _applet_Configure(null, null);

                    return;
                }

                if (_exitQuestion)
                {
                    _currentPageIndex = 0;
                    _exitQuestion = false;

                    ConsoleReporter.Report("_EXITQUESTION disabled", ConsoleReporter.ReportStatus.None, _logEnabled);

                    return;
                }
                else
                {
                    _currentPageIndex = 1;
                    _exitQuestion = true;

                    ConsoleReporter.Report("_EXITQUESTION enabled", ConsoleReporter.ReportStatus.None, _logEnabled);

                    return;
                }
            }
        }
        #endregion Events

        #region Graphics
        #region Graphic Data

        static List<LcdGdiPage> pageList;

        static DrawPages.ScrollView sview;
        static DrawPages.MessageBox_YesNo msgBox;

        static bool _exitQuestion;
        static int _currentPageIndex = 0;
        static bool _inConfigMode;

        #endregion Graphic Data

        static void InitGraphics(LcdDevice device)
        {
            ConsoleReporter.Report("Initializing Graphics...", ConsoleReporter.ReportStatus.Process, _logEnabled);
            pageList = new List<LcdGdiPage>();

            Draw(device);
            device.CurrentPage = pageList[0];
            device.SetAsForegroundApplet = true;
            ConsoleReporter.Report("Initializing Graphics done", ConsoleReporter.ReportStatus.Info, _logEnabled);
        }

        static void Draw(LcdDevice device)
        {
            sview = new DrawPages.ScrollView(appList, _configButtonEnabled);
            msgBox = new DrawPages.MessageBox_YesNo("QUIT?", "Would you like to quit\nG15 AppStart?");

            pageList.Add(sview.GetPage(device));
            pageList.Add(msgBox.GetPage(device));
            pageList.Add(DrawPages.ConfigScreen.GetPage(device));

            AssignUpdateToAllPages();

            ConsoleReporter.Report("Drawn", ConsoleReporter.ReportStatus.None, _logEnabled);
        }

        static void AssignUpdateToAllPages()
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                pageList[i].Updating += page_Updating;
            }
        }

        static void page_Updating(object sender, UpdateEventArgs e)
        {
            try
            {
                _device.CurrentPage = pageList[_currentPageIndex];
            }
            catch
            {
                _device.CurrentPage = pageList[0];
            }

            //ConsoleReporter.Report(String.Format("Frame rendered... (SinceLastFrame {0}ms, TotalTime {1}ms)", e.ElapsedTimeSinceLastFrame, e.ElapsedTotalTime));
        }
        #endregion Graphics
    }
}
