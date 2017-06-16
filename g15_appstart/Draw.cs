using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GammaJul.LgLcd;

namespace G15_AppStart
{
    static class DrawPages
    {
        public static class AboutPage
        {
            public static LcdGdiText lblAboutInfo
            {
                get
                {
                    LcdGdiText lbl = new LcdGdiText();
                    lbl.Margin = new MarginF(0, 0, 0, 0);
                    lbl.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                    lbl.Text = Constants.APP_INFO;

                    return lbl;
                }
            }
        }

        public class MessageBox_YesNo
        {
            private LcdGdiText _title;
            private LcdGdiText _message;
            private LcdGdiImage _controlYes;
            private LcdGdiImage _controlNo;

            public MessageBox_YesNo(string title, string message)
            {
                _title = new LcdGdiText();
                _title.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Bold);
                _title.Margin = new MarginF(0, 0, 0, 0);
                _title.Text = title;

                _message = new LcdGdiText();
                _message.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Regular);
                _message.Margin = new MarginF(0, 10, 0, 0);
                _message.Text = message;

                _controlYes = new LcdGdiImage(Properties.Resources.BottomScreenCommand_OK);
                _controlYes.Margin = new MarginF(0, 33, 0, 0);

                _controlNo = new LcdGdiImage(Properties.Resources.BottomScreenCommand_Cancel);
                _controlNo.Margin = new MarginF(120, 33, 0, 0);
            }

            public LcdGdiPage GetPage(LcdDevice device)
            {
                LcdGdiPage page = new LcdGdiPage(device);
                page.Children.AddRange(new LcdGdiObject[] { _title, _message, _controlYes, _controlNo });

                return page;
            }
        }

        public static class ConfigScreen
        {
            public static LcdGdiPage GetPage(LcdDevice device)
            {
                LcdGdiPage page = new LcdGdiPage(device);

                /*LcdGdiText title = new LcdGdiText();
                title.Text = "Config Mode";
                title.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Bold);
                title.Margin = new MarginF(0, 0, 0, 0);*/

                LcdGdiText message = new LcdGdiText();
                message.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Regular);
                message.Margin = new MarginF(0, 3, 0, 0);
                message.Text = "G15 AppStart is currently locked.\nTo continue operation\nclose Config application.";

                page.Children.AddRange(new LcdGdiObject[] { /*title,*/ message });

                return page;
            }
        }

        public class ScrollView
        {
            private LcdGdiText _item1;
            private LcdGdiText _item2;
            private LcdGdiText _item3;
            private LcdGdiImage _imgSelector;
            private LcdGdiImage _imgControls0;
            private LcdGdiImage _imgControls1;
            private LcdGdiImage _imgControls2;
            private LcdGdiImage _imgControls3;

            private int selectorIndex = 0;

            private List<libAppStart.App> _appList;
            private bool _enabled = true;
            private bool _configButton = false;

            public ScrollView(List<libAppStart.App> appList, bool configEnabled)
            {
                // Get Data
                _appList = appList;
                _configButton = configEnabled;

                // Init Labels
                _item1 = new LcdGdiText();
                _item2 = new LcdGdiText();
                _item3 = new LcdGdiText();

                _item1.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Regular);
                _item2.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Regular);
                _item3.Font = new Font("Microsoft Sans Serif", 7.5f, FontStyle.Regular);

                _item1.Margin = new MarginF(10, 0, 0, 0);
                _item2.Margin = new MarginF(10, 11, 0, 0);
                _item3.Margin = new MarginF(10, 22, 0, 0);

                // Fill In Data
                FillData(0);

                // Init Images
                _imgSelector = new LcdGdiImage(Properties.Resources.ArrowRight_Inverted);
                _imgSelector.Margin = new MarginF(0, 0, 0, 0);

                _imgControls0 = new LcdGdiImage(Properties.Resources.BottomScreenCommand_Up);
                _imgControls0.Margin = new MarginF(0, 33, 0, 0);
                _imgControls1 = new LcdGdiImage(Properties.Resources.BottomScreenCommand_Down);
                _imgControls1.Margin = new MarginF(40, 33, 0, 0);
                _imgControls2 = new LcdGdiImage(Properties.Resources.BottomScreenCommand_OK);
                _imgControls2.Margin = new MarginF(80, 33, 0, 0);

                _imgControls3 = _4thButton(_configButton);
            }

            private LcdGdiImage _4thButton(bool enabled)
            {
                LcdGdiImage img = new LcdGdiImage();
                if (enabled)
                    img.Image = Properties.Resources.BottomScreenCommand_Config;
                else 
                    img.Image = Properties.Resources.BottomScreenCommand_Cancel;
                
                img.Margin = new MarginF(120, 33, 0, 0);
                return img;
            }

            bool FillData(int startindex)
            {
                if (_appList == null)
                {
                    _item2.Text = "Failed to load app list!";
                    _enabled = false;
                    return false;
                }
                else if (_appList.Count == 0)
                {
                    _item2.Text = "NO ITEMS FOUND!";
                    _enabled = false;
                    return false;
                }

                _item1.Text = (startindex > _appList.Count - 1) ? "" : _appList[startindex].Name;
                _item2.Text = (startindex + 1 > _appList.Count - 1) ? "" : _appList[startindex + 1].Name;
                _item3.Text = (startindex + 2 > _appList.Count - 1) ? "" : _appList[startindex + 2].Name;

                return !String.IsNullOrWhiteSpace(_item1.Text);
            }

            public List<libAppStart.App> AppList
            {
                get { return _appList; }
                set { _appList = value; }
            }

            public void MoveSelectorDown()
            {
                if (!_enabled) return;

                if (selectorIndex + 1 >= _appList.Count) return;

                switch (selectorIndex % 3)
                {
                    case 0:
                        if (!String.IsNullOrWhiteSpace(_item2.Text))
                            selectorIndex++;
                        break;
                    case 1:
                        if (!String.IsNullOrWhiteSpace(_item3.Text))
                            selectorIndex++;
                        break;
                    case 2:
                        if (FillData(selectorIndex + 1))
                            selectorIndex++;
                        else
                            FillData(selectorIndex - 2);
                        break;
                }

                _imgSelector.Margin = new MarginF(0, ((selectorIndex % 3) * 11), 0, 0);
            }

            public void MoveSelectorUp()
            {
                if (!_enabled) return;

                if (selectorIndex == 0) return;

                switch (selectorIndex % 3)
                {
                    case 0:
                        FillData(selectorIndex - 3);
                        selectorIndex--;
                        break;
                    case 1:
                    case 2:
                        selectorIndex--;
                        break;
                }

                _imgSelector.Margin = new MarginF(0, ((selectorIndex % 3) * 11), 0, 0);
            }

            public void ExecuteSelected()
            {
                try
                {
                    _appList[selectorIndex].StartApp();
                }
                catch (Exception ex)
                {
                    libConsoleReporter.ConsoleReporter.Report(ex.Message, libConsoleReporter.ConsoleReporter.ReportStatus.Error);
                }
            }

            public string TextSlot1 { get { return _item1.Text; } set { _item1.Text = value; } }
            public string TextSlot2 { get { return _item2.Text; } set { _item2.Text = value; } }
            public string TextSlot3 { get { return _item3.Text; } set { _item3.Text = value; } }

            public void Draw(LcdDevice device)
            {

            }

            public LcdGdiPage GetPage(LcdDevice device)
            {
                LcdGdiPage page = new LcdGdiPage(device);

                if (_enabled)
                    page.Children.AddRange(new LcdGdiObject[] { 
                        _item1, _item2, _item3, _imgSelector, _imgControls0, _imgControls1, _imgControls2, _imgControls3 
                    });
                else
                    page.Children.AddRange(new LcdGdiObject[] { 
                        _item1, _item2, _item3, _imgControls3 
                    });

                return page;
            }

            public bool ConfigButtonsEnabled { get { return _configButton; } }
        }
    }
}
