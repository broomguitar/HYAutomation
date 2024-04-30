using HYAutomation.BaseView;
using HYWindowUtils.WPF.VMBaseUtil;
using System;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Module
{
    public abstract class AbstractModule : IModule
    {
        public virtual bool IsAvailable { get; } = true;
        public virtual bool IsVerify { get; }

        public abstract int ModuleIndex { get; }

        public abstract string ModuleName { get; }

        public abstract string ModuleTitle { get; }

        public virtual Lazy<Window> UserInterface { get; }

        private ICommand _openCmd;
        public virtual ICommand OpenCmd => _openCmd = _openCmd ?? new DelegateCommand(OpenModule);
        public virtual void OpenModule()
        {
            if (IsVerify)
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
            if (UserInterface != null)
            {
                UserInterface.Value.ShowDialog();
            }
        }
        public virtual void Dispose()
        {
            if (UserInterface.Value.DataContext is NotifyPropertyObject obj)
            {
                obj.Dispose();
            }
        }
    }
}
