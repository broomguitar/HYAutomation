using HYAutomation.BaseView;
using HYCommonUtils.Logger;
using System;
using System.Threading;
using System.Windows;

namespace HYAutomation.Start
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 进程互斥锁
        /// </summary>
        private Mutex _mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "HYAutomation", out bool flag);
            if (!flag)
            {
                HYMessageBox.Show("程序已经运行!", "提示");
                Environment.Exit(9);//强关
            }
            Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {

                var main = new Core.Views.MainWindow { DataContext = new Projects.HiEdgeMind.MainViewModel()};
                main.Show();
            }
            catch (Exception ex)
            {
                LogUtil.Instance.Fatal(ex.ToString());
            }
            base.OnStartup(e);
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                string msg=string.Empty;
                if (e.Exception.InnerException == null)
                {
                    msg=("（1）发生了一个错误！请联系开发人员！" + Environment.NewLine
                                       + "（2）错误源：" + e.Exception.Source + Environment.NewLine
                                       + "（3）详细信息：" + e.Exception.Message + Environment.NewLine);
                    //+ "（4）报错区域：" + e.Exception.StackTrace);
                }
                else
                {
                    msg = ("（1）发生了一个错误！请联系开发人员！" + Environment.NewLine
                                        + "（2）错误源：" + e.Exception.InnerException.Source + Environment.NewLine
                                        + "（3）错误信息：" + e.Exception.Message + Environment.NewLine
                                        + "（4）详细信息：" + e.Exception.InnerException.Message + Environment.NewLine
                                        + "（5）报错区域：" + e.Exception.InnerException.StackTrace);
                }
                Core.LogHelper.Instance.AddLog(msg, true);
                LogUtil.Instance.Fatal(msg);
            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                LogUtil.Instance.Fatal("程序发生致命错误，将终止，请联系运营商！"+ex.ToString());
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string str = e.ExceptionObject.ToString();
                Core.LogHelper.Instance.AddLog(str, true);
                LogUtil.Instance.Fatal(str);
            }
            catch(Exception ex)
            {
                LogUtil.Instance.Fatal("程序发生致命错误，将终止" + ex.ToString());
            }
            finally
            {
                //Environment.Exit(-1);
            }
        }
    }
}
