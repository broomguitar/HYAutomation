using System;
using System.Globalization;

namespace HYAutomation.BaseView.ValueConverters
{
    public class BrushToStringConverter : BaseValueConverter<BrushToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var data = (System.Drawing.Color)new System.Drawing.ColorConverter().ConvertFromString(value.ToString());
                return data.Name;
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
