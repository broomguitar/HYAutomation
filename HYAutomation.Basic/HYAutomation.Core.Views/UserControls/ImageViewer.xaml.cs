using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HYAutomation.Core.Views.UserControls
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        public ImageViewer()
        {
            InitializeComponent();
        }


        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageDataProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer viewer)
            {
                if (e.NewValue != null)
                {
                    //viewer.restoreImg();
                }
            }
        }


        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageViewer), new PropertyMetadata(default(Stretch)));


        private Point positionMouseDown;
        private bool isMouseDown;
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid && ImageSource != null)
            {
                img.CaptureMouse();
                isMouseDown = true;
                positionMouseDown = e.GetPosition(img);
            }
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Grid && ImageSource != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed && isMouseDown)
                {
                    var position = e.GetPosition(img);
                    if (translateTransform.X < -(Grid.ActualWidth - img.ActualWidth) / 2 || (translateTransform.X < 0 && (positionMouseDown.X - position.X) > 0))
                    {
                        translateTransform.X -= positionMouseDown.X - position.X;
                    }
                    if (translateTransform.Y < -(Grid.ActualHeight - img.ActualHeight) / 2 || (positionMouseDown.Y - position.Y) > 0)
                    {
                        translateTransform.Y -= positionMouseDown.Y - position.Y;
                    }
                    positionMouseDown = position;
                }
            }
        }
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid && ImageSource != null)
            {
                img.ReleaseMouseCapture();
                isMouseDown = false;
            }
        }
        private Point ImgCenterPoint;
        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is Grid && ImageSource != null)
            {
                var point = e.GetPosition(img);
                var delta = e.Delta * 0.001;
                Point pointToContent;
                if (translateTransform.X > -(Grid.ActualWidth - img.ActualWidth) / 2)
                {
                    point = new Point(img.ActualWidth / 2, point.Y);
                }
                if (translateTransform.Y > -(Grid.ActualHeight - img.ActualHeight) / 2)
                {
                    point = new Point(point.X, img.ActualHeight / 2);
                }
                pointToContent = group.Inverse.Transform(point);
                if (scaleTransform.ScaleX + delta <= 1 || scaleTransform.ScaleY + delta >= 50) return;
                scaleTransform.ScaleX += delta;
                scaleTransform.ScaleY += delta;
                translateTransform.X = -1 * ((pointToContent.X * scaleTransform.ScaleX) - point.X);
                translateTransform.Y = -1 * ((pointToContent.Y * scaleTransform.ScaleY) - point.Y);
            }

        }

        private void rotate_Click(object sender, RoutedEventArgs e)
        {
            if (ImageSource != null)
            {
                rotateTransform.CenterX = img.ActualWidth / 2;
                rotateTransform.CenterY = img.ActualHeight / 2;
                if (rotateTransform.Angle == 360)
                {
                    rotateTransform.Angle = 0;
                }
                rotateTransform.Angle += 90;
            }
        }

        private void resotre_Click(object sender, RoutedEventArgs e)
        {
            if (ImageSource != null)
            {
                restoreImg();
            }
        }

        private void enlarge_Click(object sender, RoutedEventArgs e)
        {
            if (ImageSource != null)
            {
                var point = new Point(img.ActualWidth / 2, img.ActualHeight / 2);
                var pointToContent = group.Inverse.Transform(point);
                if (scaleTransform.ScaleY >= 50) return;
                scaleTransform.ScaleX += 0.01;
                scaleTransform.ScaleY += 0.01;
                translateTransform.X = -1 * ((pointToContent.X * scaleTransform.ScaleX) - point.X);
                translateTransform.Y = -1 * ((pointToContent.Y * scaleTransform.ScaleY) - point.Y);
            }

        }

        private void shrink_Click(object sender, RoutedEventArgs e)
        {
            if (ImageSource != null)
            {
                var point = new Point(img.ActualWidth / 2, img.ActualHeight / 2);
                var pointToContent = group.Inverse.Transform(point);
                if (scaleTransform.ScaleX <= 1) return;
                scaleTransform.ScaleX -= 0.01;
                scaleTransform.ScaleY -= 0.01;
                translateTransform.X = -1 * ((pointToContent.X * scaleTransform.ScaleX) - point.X);
                translateTransform.Y = -1 * ((pointToContent.Y * scaleTransform.ScaleY) - point.Y);
            }
        }
        private void restoreImg()
        {
            scaleTransform.ScaleX = 1d;
            scaleTransform.ScaleY = 1d;
            translateTransform.X = 0d;
            translateTransform.Y = 0d;
        }
    }
}
