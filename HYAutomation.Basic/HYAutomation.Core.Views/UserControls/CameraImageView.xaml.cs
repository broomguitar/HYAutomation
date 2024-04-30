using System.Windows.Controls;
using System.Windows.Input;

namespace HYAutomation.Core.Views.UserControls
{
    /// <summary>
    /// CameraImageView.xaml 的交互逻辑
    /// </summary>
    public partial class CameraImageView : UserControl
    {
        public CameraImageView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is Grid grid && grid.DataContext is Models.CameraResultModel model)
                {
                    if (System.IO.File.Exists(model?.CameraImagePath))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(model.CameraImagePath);
                        }
                        catch
                        {
                        }

                    }
                }
            }
        }
    }
}
