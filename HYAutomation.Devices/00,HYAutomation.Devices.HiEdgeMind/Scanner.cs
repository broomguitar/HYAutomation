using HYAutomation.Core;
using HYAutomation.Device;

namespace HYAutomation.Devices.HiEdgeMind
{
    public class Scanner : AbstractScanner
    {
        public override int DeviceIndex { get; set; }
        public override bool IsAvailable { get; set; } = true;
        public override bool IsSerailPort { get; set; }
        public override string Com_Scanner { get; set; }
        public override int Baudrate_Scanner { get; set; }
        public override string IP_Scanner { get; set; }
        public override int Port_Scanner { get; set; }
    }
}
