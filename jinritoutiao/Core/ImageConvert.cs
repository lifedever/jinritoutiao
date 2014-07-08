using System;
using System.Diagnostics;
using System.Globalization;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace jinritoutiao.Core
{
    public class ImageConvert : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ImageSource imageSource = null;
            try
            {
                imageSource = new BitmapImage(new Uri(value.ToString(), UriKind.Absolute));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}