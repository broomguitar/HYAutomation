using HYAutomation.Core;
using HYAutomation.Device;
using System.Net;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class Robot : AbstractSocketServer
    {
        public override int DeviceIndex { get; set; }
        public override IPAddress IP_Server { get; set; }
        public override int Port_Server { get; set; }
        public override string DeviceName { get; set; }
        public override string DeviceDesc { get; set; }
        public override DeviceEnum DeviceEnum { get; } = DeviceEnum.Robot;
    }
}
