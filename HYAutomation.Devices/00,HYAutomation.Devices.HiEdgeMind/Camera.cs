using HYAutomation.Core;
using HYAutomation.Device;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class Camera : AbstractCamera
    {
        public override int DeviceIndex { get; set; }
        public override bool IsAvailable { get; set; } = true;
        #region 相机配置
        public override string CameraFactory { get; set; }
        public override bool IslinearCamera { get; set; }
        public override string CameraSN { get; set; }
        public override string DeviceName { get; set; }
        public override string DeviceDesc { get; set; }
        public override bool CanSetTrigger { get; set; } = true;
        #endregion
    }
}
