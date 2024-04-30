using HYAutomation.BaseView;
using HYAutomation.Core.Algorithm.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Views
{
    /// <summary>
    /// SelectDetectItemView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectDetectItemTypeView : Window
    {
        public SelectDetectItemTypeView()
        {
            InitializeComponent();
            Width = 500 / Core.DPIUtil.GetScalingRatioX();
            Height = 300 / Core.DPIUtil.GetScalingRatioY();
            Owner = System.Windows.Application.Current.MainWindow;
        }
        /// <summary>
        /// 检测点名称
        /// </summary>
        public List<DetectItemConfigModel> DetectItemConfigs { get; set; } = Algorithm.AlgorithmConfig.Instance.DetectItemConfigs;
        private DetectItemConfigModel _selectedDetectItem;
        public DetectItemConfigModel SelectedDetectItem
        {
            get
            {
                return _selectedDetectItem;
            }
            set
            {
                _selectedDetectItem = value;
            }
        }

        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDetectItem == null)
            {
                HYMessageBox.Show("条目不能为空");
                return;
            }
            this.DialogResult = true;
        }

        private void HYButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
