using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供
using jinritoutiao.Core;
using jinritoutiao.Core.Model;

namespace jinritoutiao
{
    public sealed partial class HeaderControl : UserControl
    {
        public ObservableCollection<HeaderMenu> HeaderMenus { get; set; }
        public string CurrentTime { get; set; }
        public double Height { get; set; }


        public HeaderControl()
        {
            this.InitializeComponent();
            HeaderMenus = ToutiaoHelper.GetHeaderMenus();
            CurrentTime = DateTime.Now.ToString("yyyy.MM.dd dddd");
            this.DataContext = this;
            MenuListBox.DataContext = this;
            DatetimeTextBlock.DataContext = this;
            Height = 100;
        }

      
    }
}
