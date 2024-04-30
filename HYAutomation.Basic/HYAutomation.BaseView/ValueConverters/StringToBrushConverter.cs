using System;
using System.Globalization;

namespace HYAutomation.BaseView.ValueConverters
{
    public class StringToBrushConverter : BaseValueConverter<StringToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString(value.ToString());
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class StringToColorConverter : BaseValueConverter<StringToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (System.Drawing.Color)new System.Drawing.ColorConverter().ConvertFromString(value.ToString());
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
