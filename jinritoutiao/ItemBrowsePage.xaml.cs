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
using HtmlAgilityPack;
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
            ProgressRing.IsActive = true;

            if (SettingsHelper.GetYejianState() && !SettingsHelper.GetZhuanmaState()&&SettingsHelper.GetNewsTipState())
            {
                MessageDialog msgDialog = new MessageDialog("系统检测到您启用了夜间模式，" +
                                                          "但并未启用“优化新闻查看页面”功能，" +
                                                          "这样会导致夜间模式失效，" +
                                                          "强烈建议您开启此功能！\n是否关闭此提示？", "注意：");
                //OK Button
                UICommand okBtn = new UICommand("关闭");
                okBtn.Invoked = command => SettingsHelper.ChangeToNewsTip(false);
                msgDialog.Commands.Add(okBtn);

                //Cancel Button
                UICommand cancelBtn = new UICommand("不关闭");
                cancelBtn.Invoked = command => { };
                msgDialog.Commands.Add(cancelBtn);
                msgDialog.ShowAsync();
            }

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
            Debug.WriteLine(_receiveData.SourceUrl);

            if (_receiveData != null)
            {
                if (SettingsHelper.GetZhuanmaState())
                {
                    ViewWithZhuanma();
                }
                else
                {
                    await ViewWithoutZhuanma();
                }
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

        private async Task ViewWithoutZhuanma()
        {
            string sourceUrl = _receiveData.SourceUrl;
            if (sourceUrl.StartsWith("http://toutiao.com") || sourceUrl.StartsWith("http://web.toutiao.com"))
            {
                HtmlDocument doc = new HtmlDocument();
                HttpClient client = new HttpClient();
                string content = await client.GetStringAsync(sourceUrl);
                doc.LoadHtml(content);
                var findNode =
                    doc.DocumentNode.Descendants("div")
                        .Where(
                            n =>
                                n.Attributes.Contains("class") &&
                                n.Attributes["class"].Value.Contains("article-content"));
                if (findNode != null && findNode.FirstOrDefault() != null)
                {
                    string html = findNode.FirstOrDefault().InnerHtml;
                    html = string.Format("<div><h2>{0}</h2><div style='font-size:18px;'>{1}</div></div>", _receiveData.Title, html);
                    ItemWebView.DefaultBackgroundColor = Colors.WhiteSmoke;
                    ItemWebView.NavigateToString(html);
                }
                else
                {
                    ItemWebView.Navigate(new Uri(_receiveData.SourceUrl));
                }
            }
            else
            {
                ItemWebView.Navigate(new Uri(_receiveData.SourceUrl));
            }
        }

        private async void ViewWithZhuanma()
        {
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
            Debug.WriteLine(content);
            ItemWebView.NavigateToString(content);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }
        private void ItemWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
           
        }

        private async void ItemWebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ProgressRing.IsActive = false;
            //await ItemWebView.InvokeScriptAsync("eval", new string[] { GetJs() });
            //YejianGrid.Visibility = Visibility.Collapsed;
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
            CommandBar.RequestedTheme = ElementTheme.Dark;
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

    }
}
