using HYAutomation.Core;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.BaseView
{
    /// <summary>
    /// HYMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class HYMessageBox : Window
    {
        public HYMessageBox()
        {
            InitializeComponent();
            Width = 800 / DPIUtil.GetScalingRatioX();
            Height = 450 / DPIUtil.GetScalingRatioY();
            if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsLoaded)
            {
                Owner = Application.Current.MainWindow;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }
        private static HYMessageBox _HYMessageBox;
        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            ret = MessageBoxResult.Yes;
            Close();
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Btn_No_Click(object sender, RoutedEventArgs e)
        {
            ret = MessageBoxResult.No;
            Close();
        }

        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            ret = MessageBoxResult.No;
            Close();
        }
        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public static void Show(string msg, string title = "提示")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_HYMessageBox != null)
                {
                    _HYMessageBox.Close();
                }
                _HYMessageBox = new HYMessageBox();
                _HYMessageBox.lb_title.Content = title;
                _HYMessageBox.tb_Msg.Text = msg;
                _HYMessageBox.ShowDialog();
            });
        }
        private static MessageBoxResult ret;
        public static MessageBoxResult Show(string msg, string title, MessageBoxButton _)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_HYMessageBox != null)
                {
                    _HYMessageBox.Close();
                }
                _HYMessageBox = new HYMessageBox();
                _HYMessageBox.Btn_Yes.Visibility = Visibility.Visible;
                _HYMessageBox.Btn_No.Visibility = Visibility.Visible;
                _HYMessageBox.Btn_OK.Visibility = Visibility.Hidden;
                _HYMessageBox.lb_title.Content = title;
                _HYMessageBox.tb_Msg.Text = msg;
                _HYMessageBox.ShowDialog();
            });
            return ret;
        }
    }
}
