using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI;
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
using jinritoutiao.Common;
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
        private readonly NavigationHelper _navigationHelper;
        public ItemBrowsePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this._navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState; InitStatusBar();
        }

        private void InitStatusBar()
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }
        
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            this._navigationHelper.OnNavigatedTo(e);
            var state = SettingsHelper.GetYejianState();
            if (state)
            {
                ChangeToYejian();
            }
            else
            {
                ChangeToBaitian();
            }

            _receiveData = (ReceiveData)e.Parameter;
            if (_receiveData != null)
            {
                //UrlTextBlock.Text = _receiveData.SourceUrl;
                HttpClient client = new HttpClient();
                Uri uri = new Uri(string.Format("http://h2w.iask.cn/h2wdisplay.php?u={0}&psize=4000", _receiveData.SourceUrl), UriKind.Absolute);
                string content = await client.GetStringAsync(uri);
                if (SettingsHelper.GetYejianState())
                {
                    content = content.Replace("eef4fa", "404040");
                    ItemWebView.DefaultBackgroundColor = Colors.DimGray;
                }
                content = content.Replace("href=\"/h2wdisplay.php?", "href=\"http://h2w.iask.cn/h2wdisplay.php?");
                if (content.Contains("<div class=\"footer\">"))
                    content = content.Substring(0, content.IndexOf("<div class=\"footer\">"));
                //Debug.WriteLine(content);
                //string share = "<div class=\"bshare-custom\"><a title=\"分享到新浪微博\" class=\"bshare-sinaminiblog\"></a><a title=\"分享到腾讯微博\" class=\"bshare-qqmb\"></a><a title=\"分享到微信\" class=\"bshare-weixin\"></a><a title=\"更多平台\" class=\"bshare-more bshare-more-icon more-style-sharethis\"></a><span class=\"BSHARE_COUNT bshare-share-count\">0</span></div><script type=\"text/javascript\" charset=\"utf-8\" src=\"http://static.bshare.cn/b/buttonLite.js#style=-1&amp;uuid=&amp;pophcol=3&amp;lang=zh\"></script><script type=\"text/javascript\" charset=\"utf-8\" src=\"http://static.bshare.cn/b/bshareC0.js\"></script>";
                Debug.WriteLine(content);
                ItemWebView.NavigateToString(content);
                //ItemWebView.Navigate(uri);
            }

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
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
            Launcher.LaunchUriAsync(new Uri(_receiveData.SourceUrl, UriKind.Absolute));
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

        private void ChangeToYejian()
        {
            this.RequestedTheme = ElementTheme.Dark;
            CommandBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
            CommandBar.RequestedTheme = ElementTheme.Dark; ;
        }

        private void ChangeToBaitian()
        {
            this.RequestedTheme = ElementTheme.Light;
            CommandBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
            CommandBar.RequestedTheme = ElementTheme.Light;;
        }

        private void FavoriteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (FavoriteAppBarButton.IsChecked == true)
            {
                SaveFavorite();
                ToutiaoHelper.ShowMessage("已添加到收藏！", MessageTextBlock, MyFlyout, this);
            }
            else
            {
                RemoveFavorite();
                ToutiaoHelper.ShowMessage("已从收藏中移除！", MessageTextBlock, MyFlyout, this);
            }
            FavoriteAppBarButton.Label = FavoriteAppBarButton.IsChecked == true ? "已收藏" : "收藏";
        }

        private void MyFavoriteAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavoritePage));
        }

        private void ItemWebView_OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            
        }
    }
}
