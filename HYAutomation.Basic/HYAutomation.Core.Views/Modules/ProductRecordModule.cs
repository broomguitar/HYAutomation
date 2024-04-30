using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class ProductRecordModule : AbstractModule
    {
        public override int ModuleIndex => 1;
        public override string ModuleName => "ProductRecordModule";

        public override string ModuleTitle => "记录查询";
        public override Lazy<Window> UserInterface => new Lazy<Window>();
    }
}
