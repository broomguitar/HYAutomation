using HYAutomation.BaseView.ValueConverters;
using System;
using System.Globalization;

namespace HYAutomation.Core.Views.ValueConverters
{
    public class FileToImageConverter : BaseValueConverter<FileToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string imgPath = value.ToString();
                if (!string.IsNullOrEmpty(imgPath))
                {
                    return Core.ImageHelper.GetBitmapImage(imgPath);
                }
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
