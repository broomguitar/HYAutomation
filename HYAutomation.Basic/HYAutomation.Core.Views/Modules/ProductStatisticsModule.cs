using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class ProductStatisticsModule : AbstractModule
    {
        public override bool IsAvailable => false;
        public override int ModuleIndex => 0;
        public override string ModuleName => "ProductStatisticsModule";

        public override string ModuleTitle => "生产统计";

        public override Lazy<Window> UserInterface => new Lazy<Window>();
    }
}
