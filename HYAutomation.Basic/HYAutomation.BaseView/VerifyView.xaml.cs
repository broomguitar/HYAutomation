using System.Windows;
using System.Windows.Input;

namespace HYAutomation.BaseView
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyView : Window
    {
        public VerifyView()
        {
            InitializeComponent();
            Width = 600 / Core.DPIUtil.GetScalingRatioX();
            Height = 400 / Core.DPIUtil.GetScalingRatioY();
            Owner = System.Windows.Application.Current.MainWindow;
            pwd.Focus();
        }
        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void HYButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (HYCommonUtils.EncryptUtils.MD5Util.MD5Encrypt32(pwd.Password.Trim()).Equals("CFC776CB1F2E44F51BB139D0A23FA0"))
            {
                this.DialogResult = true;
            }
            else
            {
                HYMessageBox.Show("密码不正确，请重新输入!");
            }
        }
    }
}
