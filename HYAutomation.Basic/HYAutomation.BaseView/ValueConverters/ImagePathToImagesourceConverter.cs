using System;
using System.Globalization;

namespace HYAutomation.BaseView.ValueConverters
{
    public class ImagePathToImagesourceConverter : BaseValueConverter<ImagePathToImagesourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return Core.ImageHelper.GetBitmapImage(value.ToString());
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
