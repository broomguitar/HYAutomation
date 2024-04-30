using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Views
{
    /// <summary>
    /// ProductRecordView.xaml 的交互逻辑
    /// </summary>
    public partial class BaseProductRecordView : Window
    {
        public BaseProductRecordView()
        {
            InitializeComponent();
            Width = 1600 / Core.DPIUtil.GetScalingRatioX();
            Height = 900 / Core.DPIUtil.GetScalingRatioY();
            Owner = Application.Current.MainWindow;
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
