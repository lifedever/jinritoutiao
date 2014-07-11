﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace jinritoutiao.Core
{
    public class AbstractStateConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (SettingsHelper.GetAbstractState())
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
