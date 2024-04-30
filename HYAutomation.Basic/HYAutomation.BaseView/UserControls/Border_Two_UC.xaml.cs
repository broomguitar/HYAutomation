using System.Windows;
using System.Windows.Controls;

namespace HYAutomation.BaseView.UserControls
{
    /// <summary>
    /// Border_Two_UC.xaml 的交互逻辑
    /// </summary>
    public partial class Border_Two_UC : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public Border_Two_UC()
        {
            this.InitializeComponent();
        }

        public string PanelTitle
        {
            get { return (string)GetValue(PanelTitleProperty); }
            set { SetValue(PanelTitleProperty, value); }
        }

        public static readonly DependencyProperty PanelTitleProperty =
          DependencyProperty.RegisterAttached("PanelTitle", typeof(string), typeof(Border_Two_UC),
              new FrameworkPropertyMetadata("标题名称", new PropertyChangedCallback(OnPanelTitlePropertyChanged)));

        public static void OnPanelTitlePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Border_Two_UC dev = sender as Border_Two_UC;
            dev.PanelTitle = (string)e.NewValue;
            dev.lblTitle.Content = (string)e.NewValue;
        }
    }
}
