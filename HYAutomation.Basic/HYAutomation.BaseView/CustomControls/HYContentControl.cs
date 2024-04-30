using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HYAutomation.BaseView.CustomControls
{
    public class HYContentControl : ContentControl
    {
        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register("TitleContent", typeof(object), typeof(HYContentControl), new PropertyMetadata("标题名称"));


        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(HYContentControl), new PropertyMetadata(24d));


        public Brush TitleBackground
        {
            get { return (Brush)GetValue(TitleBackgroundProperty); }
            set { SetValue(TitleBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(HYContentControl), new PropertyMetadata(default(Brush)));




        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(HYContentControl), new PropertyMetadata(default(Brush)));


        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleMarginProperty =
            DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(HYContentControl), new PropertyMetadata(new Thickness(20, 10, 20, 10d)));




    }
}
