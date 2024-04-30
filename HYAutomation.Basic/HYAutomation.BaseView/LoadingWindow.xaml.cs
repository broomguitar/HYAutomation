using HYAutomation.Core;
using System.Windows;

namespace HYAutomation.BaseView
{
    /// <summary>
    /// LoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            this.Width = 380 / DPIUtil.GetScalingRatioX();
            this.Height = 380 / DPIUtil.GetScalingRatioY();
        }
        public LoadingWindow(string text) : this()
        {
            tb.Text = text;
        }
    }
}
