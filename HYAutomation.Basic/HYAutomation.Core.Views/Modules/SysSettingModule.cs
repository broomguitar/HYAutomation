using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class SysSettingModule : AbstractModule
    {
        public override bool IsAvailable => false;
        public override int ModuleIndex => 4;
        public override string ModuleName => "SysSettingModule";

        public override string ModuleTitle => "系统设置";
        public override Lazy<Window> UserInterface => new Lazy<Window>();
    }
}
