using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class UserManagementModule : AbstractModule
    {
        public override bool IsAvailable => false;
        public override int ModuleIndex => 3;
        public override string ModuleName => "UserManagementModule";
        public override string ModuleTitle => "用户管理";
        public override Lazy<Window> UserInterface => new Lazy<Window>();
    }
}
