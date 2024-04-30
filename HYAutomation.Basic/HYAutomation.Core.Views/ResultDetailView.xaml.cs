using HYAutomation.BaseView;
using HYAutomation.Core.Views.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HYAutomation.Core.Views
{
    /// <summary>
    /// ResultDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class ResultDetailView : Window
    {
        public ResultDetailView()
        {
            InitializeComponent();
            Width = 800 / Core.DPIUtil.GetScalingRatioX();
            Height = 900 / Core.DPIUtil.GetScalingRatioY();
            Owner = System.Windows.Application.Current.MainWindow;
        }
        public ResultDetailView(ProductInfoModel productInfoModel) : this()
        {
            try
            {
                Product = productInfoModel;
                if (Product != null && Product.CameraDatas != null)
                {
                    foreach (var item in Product.CameraDatas)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.CameraImagePath))
                            {
                                BitmapSource bitmapImage = Core.ImageHelper.GetBitmapImage(item.CameraImagePath);
                                item.CameraImage = bitmapImage;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
           
            }
        }
        public ProductInfoModel Product { get; set; }
        private void Grid_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Product != null && Product.CameraDatas != null)
                {
                    foreach (var item in Product.CameraDatas)
                    {
                        if (item != null)
                        {
                            item.CameraImage = null;
                            item.Dispose();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString());
            }
            finally
            {
                GC.Collect();
                Close();
            }
        }
    }
}
