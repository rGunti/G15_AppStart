using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using GammaJul.LgLcd;

namespace lbG15Graphics
{
    public class G15MonoApplet
    {
        private readonly Random _random = new Random();
        private readonly AutoResetEvent _waitAre = new AutoResetEvent(false);
        private volatile bool _monoArrived;
        private volatile bool _mustExit;

        private LcdDevice _device;
        private LcdApplet _applet;

        public EventHandler OnConfig, OnEnableChange, OnDraw;
        public EventHandler<LcdDeviceTypeEventArgs> OnDeviceArrival, OnDeviceRemoval;
        public EventHandler<LcdSoftButtonsEventArgs> OnButtonPress;

        private List<G15AppletPage> _pageList;

        public G15MonoApplet(string appletName)
        {
            InitApplet(appletName);
            _pageList = new List<G15AppletPage>();
        }

        void InitApplet(string appletName)
        {
            _applet = new LcdApplet(appletName);

            _applet.Configure += _applet_Configure;
            _applet.DeviceArrival += _applet_DeviceArrival;
            _applet.DeviceRemoval += _applet_DeviceRemoval;
            _applet.IsEnabledChanged += _applet_IsEnabledChanged;
        }

        #region Events
        private void TriggerEvent(EventHandler e)
        {
            if (e != null)
                e(this, EventArgs.Empty);
        }

        private void TriggerEvent(EventHandler<LcdDeviceTypeEventArgs> e)
        {
            if (e != null)
                e(this, new LcdDeviceTypeEventArgs(LcdDeviceType.Monochrome));
        }

        private void TriggerEvent(EventHandler<LcdSoftButtonsEventArgs> e, LcdSoftButtons buttons)
        {
            if (e != null)
                e(this, new LcdSoftButtonsEventArgs(buttons));
        }

        void _applet_IsEnabledChanged(object sender, EventArgs e)
        {
            TriggerEvent(OnEnableChange);
        }

        void _applet_DeviceRemoval(object sender, LcdDeviceTypeEventArgs e)
        {
            TriggerEvent(OnDeviceRemoval);
        }

        void _applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            TriggerEvent(OnDeviceArrival);
        }

        void _applet_Configure(object sender, EventArgs e)
        {
            TriggerEvent(OnConfig);
        }

        void _device_SoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            TriggerEvent(OnButtonPress, e.SoftButtons);
        }
        #endregion Events

        public void Run()
        {
            if (_applet == null) return;

            _applet.Connect();

            _device = null;
            _waitAre.WaitOne();

            do
            {
                if (_monoArrived)
                {
                    _device = (LcdDeviceMonochrome)_applet.OpenDeviceByType(LcdDeviceType.Monochrome);
                    _device.SoftButtonsChanged += _device_SoftButtonsChanged;
                    Draw();
                }

                if (_applet.IsEnabled && _device != null && !_device.IsDisposed)
                    _device.DoUpdateAndDraw();
                
            } while (!_mustExit);

            if (_applet.IsEnabled && _device != null && !_device.IsDisposed)
                _device.DoUpdateAndDraw();

            Thread.Sleep(5);
        }

        void Draw()
        {

        }
    }
}
