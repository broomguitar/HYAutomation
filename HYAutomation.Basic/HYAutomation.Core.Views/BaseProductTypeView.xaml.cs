using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Views
{
    /// <summary>
    /// TypeSettings.xaml 的交互逻辑
    /// </summary>
    public partial class BaseProductTypeView : Window
    {
        public BaseProductTypeView()
        {
            InitializeComponent();
            this.Width = 1600 / Core.DPIUtil.GetScalingRatioX();
            this.Height = 900 / Core.DPIUtil.GetScalingRatioY();
            this.Owner = System.Windows.Application.Current.MainWindow;
        }

        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
