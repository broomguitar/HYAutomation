using HYAutomation.BaseView;
using HYAutomation.Core.Algorithm.Views;
using HYAutomation.Module;
using System;
using System.Windows;

namespace HYAutomation.Core.Views.Modules
{
    public class SolutionConfigModule : AbstractModule
    {
        public override int ModuleIndex => 4;
        public override string ModuleName => "SolutionConfigModule";

        public override string ModuleTitle => "配置方案";
        public override Lazy<Window> UserInterface => new Lazy<Window>(() => new DetectItemSettingView());
        public override void OpenModule()
        {
            if (ModuleName == "SolutionConfigModule")
            {
                var vertify = new VerifyView();
                if (vertify.ShowDialog() == true)
                {
                    if (UserInterface != null)
                    {
                        UserInterface.Value.ShowDialog();
                    }
                }
                return;
            }
            base.OpenModule();
        }
    }
}
