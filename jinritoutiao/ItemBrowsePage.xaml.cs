using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍
using jinritoutiao.Core;
using jinritoutiao.Core.Model;

namespace jinritoutiao
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ItemBrowsePage : Page
    {
        private ReceiveData _receiveData;
        public ItemBrowsePage()
        {
            this.InitializeComponent();
            InitStatusBar();
        }

        private void InitStatusBar()
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _receiveData = (ReceiveData)e.Parameter;
            if (_receiveData != null)
            {
                UrlTextBlock.Text = _receiveData.SourceUrl;
                ItemWebView.Navigate(new Uri(_receiveData.SourceUrl, UriKind.Absolute));
            }
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack) 
            {
                Frame.GoBack();
            }
        }

        private void ItemWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ProgressRing.IsActive = true;
        }

        private void ItemWebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ProgressRing.IsActive = false;
        }

        private void BackAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ItemWebView.CanGoBack)
            {
                ItemWebView.GoBack();
            }
        }

        private void RefreshAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemWebView.Refresh();
        }

        private void ForwardAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ItemWebView.CanGoForward)
            {
                ItemWebView.GoForward();
            }
        }

        private async void FavoriteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            var tempItem = App.FavoriteDatas.Find(n => n.Id == _receiveData.Id); 
            if(tempItem == null)
                App.FavoriteDatas.Add(_receiveData);
            await ToutiaoHelper.SaveFavorite();
        }
    }
}
