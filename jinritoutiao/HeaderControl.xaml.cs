using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace jinritoutiao
{
    public sealed partial class HeaderControl : UserControl
    {
        
        public HeaderControl()
        {
            this.InitializeComponent();
        }

        private void RefreshImage_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            RefreshImageStoryboard.Begin();
        }

        private void MenuListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = sender as ListViewItem;
            //Debug.WriteLine();
        }
    }
}
