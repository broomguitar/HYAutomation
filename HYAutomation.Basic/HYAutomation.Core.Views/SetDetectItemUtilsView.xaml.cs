using HYAutomation.Core.Algorithm.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HYAutomation.Core.Views
{
    /// <summary>
    /// SelectDetectItemStandardValue.xaml 的交互逻辑
    /// </summary>
    public partial class SetDetectItemUtilsView : Window
    {
        public SetDetectItemUtilsView(IEnumerable<AlgorithmUtilsModel> detectItemContents)
        {
            InitializeComponent();
            this.Width = 800 / Core.DPIUtil.GetScalingRatioX();
            this.Height = 600 / Core.DPIUtil.GetScalingRatioY();
            this.Owner = Application.Current.MainWindow;
            Detects = detectItemContents;
        }
        public IEnumerable<AlgorithmUtilsModel> Detects { get; }
        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "D:/Model/NCC";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Detects.ToList()[dg.SelectedIndex].AlgorithmUtilsValue = FileUtils.FilePathUtil.NormalizePath(openFileDialog.FileName);
            }
        }
    }
}
