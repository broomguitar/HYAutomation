using HY.Devices.Scanner;
using System;
using System.Threading.Tasks;

namespace HYAutomation.Device
{
    public abstract class AbstractScanner : AbstractDevice
    {
        #region 扫码枪配置
        public abstract bool IsSerailPort { get; set; }
        public abstract string Com_Scanner { get; set; }
        public abstract int Baudrate_Scanner { get; set; }
        public abstract string IP_Scanner { get; set; }
        public abstract int Port_Scanner { get; set; }
        #endregion
        public override string DeviceName { get; set; } = "Scanner";
        public override string DeviceDesc { get; set; } = "扫码枪";
        public override DeviceEnum DeviceEnum { get; } = DeviceEnum.Scanner;
        private IHYScanner _scanner;
        public override event EventHandler<object> PushResultEvent;

        public override bool Connect()
        {
            try
            {
                if (base.Connect())
                {
                    return true;
                }
                if (_scanner != null)
                {
                    _scanner.Close();
                    _scanner.Dispose();
                    _scanner = null;
                }
                Initialization();
                if (_scanner != null)
                {
                    _scanner.Open();
                    IsOnline = _scanner.IsConnected;
                    if (!IsOnline)
                    {
                        addDeviceLog(_scanner.GetLastError());
                    }
                }
                return IsOnline;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }

        public override bool DisConnect()
        {
            if (_scanner != null)
            {
                try
                {
                    _scanner.HasNewDataReceived -= HasNewData;
                    _scanner.Close();
                    _scanner.Dispose();
                    _scanner = null;
                    IsOnline = false;
                }
                catch (Exception ex)
                {
                    addDeviceLog(ex.ToString());
                    return false;
                }
            }
            base.Dispose();
            return true;
        }
        private Task _monitorTask;
        private Task Initialization()
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                if (IsSerailPort)
                {
                    _scanner = new HYScannerSerialPort(Com_Scanner, Baudrate_Scanner);
                }
                else
                {
                    _scanner = new HYScannerNET(IP_Scanner, Port_Scanner);
                }
                _scanner.HasNewDataReceived += HasNewData;
                if (_monitorTask == null || _monitorTask.IsCompleted)
                {
                    _monitorTask = Task.Factory.StartNew(() =>
                    {
                        while (_scanner != null)
                        {
                            try
                            {
                                IsOnline = _scanner.IsConnected;
                                System.Threading.Thread.Sleep(2000);
                            }
                            catch (Exception ex)
                            {

                                addDeviceLog(ex.ToString());
                            }

                        }
                    }, TaskCreationOptions.LongRunning);
                }
            }
            catch (Exception ex)
            {
                _scanner = null;
                addDeviceLog(ex.ToString());
            }
            finally
            {
                tcs.SetResult(true);
            }
            return tcs.Task;
        }
        private void HasNewData(object sender, string newData)
        {
            newData = newData.Trim();
            if (newData == null)
            {
                addDeviceLog("收到无效数据", false);
                return;
            }
            addDeviceLog($"收到数据{newData}", false);
            //DataQueue.Enqueue(newData);
           // PushResultEvent?.Invoke(this, newData);
            PushResultEvent?.BeginInvoke(this, newData, null, null);
        }
    }
}
