using HYAutomation.BaseView.ValueConverters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HYAutomation.Core.Views.ValueConverters
{
    public class ProductTypeDetailsToImageConverter : BaseValueConverter<ProductTypeDetailsToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is IEnumerable<Models.ProductTypeDetailModel> ls)
            {
                string imgPath = ls.FirstOrDefault()?.TemplateImagePath;
                if (!string.IsNullOrEmpty(imgPath))
                {
                    return Core.ImageHelper.GetBitmapImage(imgPath, 100);
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
