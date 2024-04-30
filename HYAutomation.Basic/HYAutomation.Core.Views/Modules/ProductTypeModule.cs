using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class ProductTypeModule : AbstractModule
    {
        public override int ModuleIndex => 2;
        public override string ModuleName => "ProductTypeModule";

        public override string ModuleTitle => "型号管理";
        public override Lazy<Window> UserInterface => new Lazy<Window>();
    }
}
