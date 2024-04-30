using HYAutomation.Core;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class _3DCamera : Abstract3DCamera
    {
        public override int DeviceIndex { get; set; }
        #region 相机配置

        public override int DeviceID { get; set; }
        public override string IP {get;set; }
        #endregion
        public override string DeviceName { get; set; }
        public override string DeviceDesc { get; set; }
    }
}
