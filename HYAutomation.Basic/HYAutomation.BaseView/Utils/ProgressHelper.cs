using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HYAutomation.BaseView.Utils
{
    public static class ProgressHelper
    {
        public static void ShowProgress(Task t)
        {
            t.Start();
            var dialog = new LoadingWindow();
            dialog.Owner = Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            t.ContinueWith(tt => Application.Current.Dispatcher.Invoke(() => { dialog.Close(); Application.Current.MainWindow.Activate(); }));
            dialog.ShowDialog();
        }
        public static void ShowProgress(Task t, string loadingText)
        {
            t.Start();
            var dialog = new LoadingWindow(loadingText);
            dialog.Owner = Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            t.ContinueWith(tt => Application.Current.Dispatcher.Invoke(() => { dialog.Close(); Application.Current.MainWindow.Activate(); }));
            dialog.ShowDialog();
        }
        private static LoadingWindow _progressbar;
        public static void Show()
        {
            Thread t = new Thread(() =>
            {
                _progressbar = new LoadingWindow();
                _progressbar.ShowDialog();//不能用Show
            });
            t.SetApartmentState(ApartmentState.STA);//设置单线程
            t.Start();
        }
        public static void Show(string loadingText)
        {
            Thread t = new Thread(() =>
            {
                _progressbar = new LoadingWindow(loadingText);
                _progressbar.ShowDialog();//不能用Show
            });
            t.SetApartmentState(ApartmentState.STA);//设置单线程
            t.Start();
        }
        public static void Close()
        {
            while (_progressbar == null)
            {
                Thread.Sleep(10);
            }
            _progressbar.Dispatcher.Invoke(() =>
            {
                _progressbar.Close();
            });
        }
        public static void DoWork(Action action)
        {
            Show();
            action?.Invoke();
            Close();
        }
        public static void DoWork(Action action, string loadingText)
        {
            Show(loadingText);
            action?.Invoke();
            Close();
        }
    }
}
