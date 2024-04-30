using HY.Devices.PLC;
using HYAutomation.BaseView;
using HYAutomation.Core;
using HYAutomation.Device;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class PLC : AbstractPLC
    {
        public override int DeviceIndex { get; set; }
        public override bool IsAvailable { get; set; } = true;
        /// <summary>
        /// 到位信号地址
        /// </summary>
        public string Addr_Arrive { get; set; }
        /// <summary>
        /// 合格信号地址值
        /// </summary>
        public string Addr_OK { get; set; }
        /// <summary>
        /// NG信号地址值
        /// </summary>
        public string Addr_NG { get; set; }
        /// <summary>
        /// 抓拍完成信号地址值
        /// </summary>
        public string Addr_GrabFinish { get; set; }
        /// <summary>
        /// 线扫相机使能信号
        /// </summary>
        public string Addr_LinearStart { get; set; }
        /// <summary>
        /// 通讯成功信号地址值
        /// </summary>
        public string Addr_CommOK { get; set; }
        public override string IP_PLC { get; set; }
        public override int Port_PLC { get; set; }
        public override string PLCFactory { get; set; } 
        private Task checkArrivalTask;
        private void InitialModule()
        {
            try
            {
                switch (PLCFactory.ToUpper())
                {
                    case "MELSEC":
                        {
                            _hyPLCComm = new HYPLCComm_MelsecNet(IP_PLC, Port_PLC);
                        }
                        break;
                    case "SIEMENS":
                        {
                            _hyPLCComm = new HYPLCComm_SiemensNet(HslCommunication.Profinet.Siemens.SiemensPLCS.S1200, IP_PLC);
                        }
                        break;
                    case "PANASONIC":
                        {
                            _hyPLCComm = new HYPLCComm_PanasonicNet(IP_PLC, Port_PLC);
                        }
                        break;
                    case "OMRON":
                        {
                            _hyPLCComm = new HYPLCComm_OmronNet(IP_PLC, Port_PLC);
                        }
                        break;
                    default:
                        {
                            _hyPLCComm = new HYPLCComm_MelsecNet(IP_PLC, Port_PLC);
                        }
                        break;
                }
                _hyPLCComm.Open();
                IsOnline = _hyPLCComm.IsConnected;
                if(!string.IsNullOrEmpty(Addr_Arrive)) 
                { 
                _hyPLCComm.Write<int>(Addr_Arrive, 0);
                checkArrivalTask = new Task(() =>
                {
                    int preData = _hyPLCComm.Read<int>(Addr_Arrive);
                    while (IsOnline)
                    {
                        int ret = _hyPLCComm.Read<int>(Addr_Arrive);
                        if (ret != preData)
                        {
                            PushResultEvent?.Invoke(this, new KeyValuePair<PlcSignalEnum, object>(PlcSignalEnum.Arrival, ret));
                        }
                        preData = ret;
                        System.Threading.Thread.Sleep(100);
                    }
                });
                checkArrivalTask.Start();
                }
            }
            catch (Exception ex)
            {
                _hyPLCComm = null;
                addDeviceLog(ex.ToString());
            }
        }
        private IHYPLCComm _hyPLCComm;
        public override string DeviceName { get; set; } = "PLC";
        public override string DeviceDesc { get; set; } = "PLC";
        public override DeviceEnum DeviceEnum { get; } = DeviceEnum.PLC;

        public override bool Connect()
        {
            try
            {
                if (base.Connect())
                {
                    return true;
                }
                if (_hyPLCComm != null)
                {
                    _hyPLCComm.Close();
                    _hyPLCComm.Dispose();
                    _hyPLCComm = null;
                }
                InitialModule();
                return _hyPLCComm?.IsConnected == true;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public override bool DisConnect()
        {
            if (_hyPLCComm != null)
            {
                try
                {
                    _hyPLCComm.Close();
                    IsOnline = false;
                    while (checkArrivalTask?.Status == TaskStatus.Running)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (!string.IsNullOrEmpty(Addr_CommOK)&&_hyPLCComm.Write(Addr_CommOK, false))
                    {
                        addDeviceLog($"写入{Addr_CommOK}值{false}---成功", false);
                    }
                    _hyPLCComm.Dispose();
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
        public override event EventHandler<object> PushResultEvent;
        public override bool SetOK()
        {
            try
            {
                if (_hyPLCComm == null || !_hyPLCComm.IsConnected) return false;
                bool b = _hyPLCComm.Write(Addr_OK, true);
                addDeviceLog($"写入{Addr_OK}值true---{b}", false);
                return b;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }

        }
        public override bool SetNG()
        {
            try
            {
                if (_hyPLCComm == null || !_hyPLCComm.IsConnected) return false;
                bool b = _hyPLCComm.Write(Addr_NG, true);
                addDeviceLog($"写入{Addr_NG}值true---{b}", false);
                return b;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }

        }
        public override bool SetLinearStart()
        {
            try
            {
                return true;
                if (_hyPLCComm == null || !_hyPLCComm.IsConnected) return false;
                bool b = _hyPLCComm.Write(Addr_LinearStart, true);
                addDeviceLog($"写入{Addr_LinearStart}值{true}---{b}", false);
                return b;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public override bool SetLinearFinish()
        {
            try
            {
                return true;
                if (_hyPLCComm == null || !_hyPLCComm.IsConnected) return false;
                bool b = _hyPLCComm.Write(Addr_GrabFinish, true);
                addDeviceLog($"写入{Addr_GrabFinish}值{true}---{b}", false);
                return b;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
        public override bool SetValue<T>(string addr, T Value)
        {
            try
            {
                if (_hyPLCComm == null || !_hyPLCComm.IsConnected) return false;
                bool b = _hyPLCComm.Write(addr, Value);
                addDeviceLog($"写入{addr}值true---{b}", false);
                return b;
            }
            catch (Exception ex)
            {
                addDeviceLog(ex.ToString());
                return false;
            }
        }
    }
}
