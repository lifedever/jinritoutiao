using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Notifications;
using Windows.UI.Popups;
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _receiveData = (ReceiveData)e.Parameter;
            if (_receiveData != null)
            {
                UrlTextBlock.Text = _receiveData.SourceUrl;
                ItemWebView.Navigate(new Uri(_receiveData.SourceUrl, UriKind.Absolute));
            }
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            List<ReceiveData> receiveDatas = await LocalFileHelper.Read<List<ReceiveData>>(ToutiaoHelper.FILE_NAME);
            if (receiveDatas != null && receiveDatas.Count > 0)
            {
                App.FavoriteDatas = receiveDatas;
            }
            else
            {
                await LocalFileHelper.Save(ToutiaoHelper.FILE_NAME, App.FavoriteDatas);
            }
            
            FavoriteAppBarButton.IsChecked = ExistItem();
            FavoriteAppBarButton.Label = FavoriteAppBarButton.IsChecked == true ? "已收藏" : "收藏";
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

        private bool ExistItem()
        {
            var tempItem = App.FavoriteDatas.Find(n => n.Id == _receiveData.Id);
            return tempItem != null;
        }

        private async void SaveFavorite()
        {
            if (!ExistItem())
                App.FavoriteDatas.Add(_receiveData);
            await LocalFileHelper.Save(ToutiaoHelper.FILE_NAME, App.FavoriteDatas);
        }

        private async void RemoveFavorite()
        {
            App.FavoriteDatas.RemoveAll(n => n.Id == _receiveData.Id);
            await LocalFileHelper.Save(ToutiaoHelper.FILE_NAME, App.FavoriteDatas);
        }

        private void FavoriteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FavoriteAppBarButton.IsChecked == true)
            {
                SaveFavorite();
                MessageDialog message = new MessageDialog("已添加到收藏！", "恭喜");
                message.ShowAsync();
            }
            else
            {
                RemoveFavorite();
                MessageDialog message = new MessageDialog("已从收藏中移除！", "恭喜");
                message.ShowAsync();
            }
            FavoriteAppBarButton.Label = FavoriteAppBarButton.IsChecked == true ? "已收藏" : "收藏";
        }

        private void MyFavoriteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavoritePage));
        }
    }
}
