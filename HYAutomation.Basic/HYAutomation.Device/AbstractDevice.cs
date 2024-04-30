using HYAutomation.BaseView;
using HYAutomation.Core;
using HYWindowUtils.WPF.IconfontUtil;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Device
{
    public abstract class AbstractDevice : NotifyPropertyObject, IDevice
    {
        public virtual string DeviceId { get; set; }
        public abstract int DeviceIndex { get; set; }
        public virtual bool IsAvailable { get; set; } = true;
        public abstract string DeviceName { get; set; }
        public abstract string DeviceDesc { get; set; }
        public virtual UIElement UserInterface { get; }

        private bool _isOnline;

        public virtual bool IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value; RaisePropertyChanged(); }
        }
        public abstract DeviceEnum DeviceEnum { get; }

        public virtual object Icon
        {
            get
            {
                try
                {
                    return ((IconfontEnum)Enum.Parse(typeof(IconfontEnum), DeviceEnum.ToString())).GetIconFontKey();
                }
                catch
                {
                    return null;
                }
            }
        }
        public virtual bool Connect()
        {
            if (IsOnline)
            {
                if (HYMessageBox.Show("该设备当前显示在线,是否继续重连", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        public abstract bool DisConnect();
        public ConcurrentQueue<object> DataQueue { get; protected set; } = new ConcurrentQueue<object>();
        public abstract event EventHandler<object> PushResultEvent;
        private ICommand _connectCmd;
        public virtual ICommand ConnectCmd => _connectCmd = _connectCmd ?? new DelegateCommand(() => { Connect(); });
        public override void Dispose()
        {
            base.Dispose();
            while (DataQueue.TryDequeue(out object d))
            {
                System.Threading.Thread.Sleep(10);
            }
        }
        protected void addDeviceLog(string msg, bool isError = true)
        {
            LogHelper.Instance.AddLog($"【{DeviceDesc}】" + msg, isError);
        }
    }
}
