using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HYAutomation.BaseView.CustomControls
{
    public class HYButton : Button
    {


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HYButton), new PropertyMetadata(new CornerRadius(3)));
        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(HYButton), new PropertyMetadata(null));



        public Brush PressedBrush
        {
            get { return (Brush)GetValue(PressedBrushProperty); }
            set { SetValue(PressedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressedBrushProperty =
            DependencyProperty.Register("PressedBrush", typeof(Brush), typeof(HYButton), new PropertyMetadata(null));


        public Brush UnabledBrush
        {
            get { return (Brush)GetValue(UnabledBrushProperty); }
            set { SetValue(UnabledBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnabledBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnabledBrushProperty =
            DependencyProperty.Register("UnabledBrush", typeof(Brush), typeof(HYButton), new PropertyMetadata(null));







    }
}
