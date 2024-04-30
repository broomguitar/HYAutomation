using System;

namespace HYAutomation.Device
{
    public abstract class AbstractPLC : AbstractDevice
    {
        #region PLC配置
        public abstract string PLCFactory { get; set; }
        public abstract string IP_PLC { get; set; }
        public abstract int Port_PLC { get; set; }
        #endregion
        public abstract bool SetOK();
        public abstract bool SetNG();
        public virtual bool SetLinearStart()
        {
            throw new NotImplementedException();
        }
        public virtual bool SetLinearFinish()
        {
            throw new NotImplementedException();
        }
        public virtual bool ReadValue<T>(string addr,out T value)
        {
            throw new NotImplementedException();
        }
        public virtual bool SetValue<T>(string addr, T Value)
        {
            throw new NotImplementedException();
        }
    }
    public enum PlcSignalEnum
    {
        Arrival,
        ConveyAutoStop,
        Alarm
    }
}
