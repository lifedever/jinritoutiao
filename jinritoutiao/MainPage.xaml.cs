using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍
using jinritoutiao.Common;
using jinritoutiao.Core;
using jinritoutiao.Core.Model;

namespace jinritoutiao
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private ListBox _headMenuListBox;
        private HeaderMenu _headerMenu;
        private double _maxBehotTime;
        private DispatcherTimer dispatcherTimer;


        public HtmlParseHelper HtmlParseHelper { get; set; }

        public ObservableCollection<ReceiveData> ReceiveDatas { get; set; }
        public Next Next { get; set; }
        
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this._navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;

            ReceiveDatas = new ObservableCollection<ReceiveData>();
            Next = new Next();
            DataListView.DataContext = this;
            
            InitStatusBar();
            //InitConfig();
            InitHeaderMenu();

            InitPopup();
            
        }

        private void InitPopup()
        {

            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey("popuped") || !(bool)settings.Values["popuped"]) 
            {
                PopupWebView.Navigate(new Uri("https://raw.githubusercontent.com/gefangshuai/jinritoutiao/master/jinritoutiao/info.html", UriKind.Absolute));
                PopupGrid.Width = Window.Current.Bounds.Width;
                PopupGrid.Height = Window.Current.Bounds.Height;
                popup.IsOpen = true;
                CommandBar.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        private async void InitConfig()
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri("http://toutiao.com/", UriKind.Absolute));
            request.Method = "GET";
            WebResponse response = await request.GetResponseAsync();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    for (int i = 0; i < 300; i++)
                    {
                        string str = reader.ReadLine();

                        if (str.Contains("var max_behot_time"))
                        {
                            _maxBehotTime = double.Parse(str.Split('\"')[1]);
                            break;
                        }
                    }
                }
            }
            _headMenuListBox.SelectedIndex = 0;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(100);
            dispatcherTimer.Tick += (sender, o) =>
            {
                _maxBehotTime += 0.01;
            };
            dispatcherTimer.Start();
        }

        #region 初始化信息
        /// <summary>
        /// 初始化菜单对象
        /// </summary>
        private void InitHeaderMenu()
        {
            _headMenuListBox = MyHeaderControl.MenuListBox;
            _headMenuListBox.SelectionChanged += HeadMenuListBox_SelectionChanged;
            _headMenuListBox.SelectedIndex = 0;
            MyHeaderControl.LoadStoryboard.Begin();
        }

        void HeadMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _headerMenu = _headMenuListBox.SelectedItem as HeaderMenu;
            if (_headerMenu != null)
            {
                ReceiveListData();
            }
        }

        private void ReloadData()
        {
            try
            {
                MyHeaderControl.RefreshImageStoryboard.Begin();
                ProgressBar.Visibility = Visibility.Visible;
                FooterGrid.Visibility = Visibility.Collapsed;
                HtmlParseHelper = new HtmlParseHelper(ReceiveDatas, Next, FooterGrid, ProgressBar);

                string maxCreateTime = null;

                if (ReceiveDatas.Count>0)
                {
                    maxCreateTime = ReceiveDatas.FirstOrDefault().CreateTime;
                }

                string url = ToutiaoHelper.GetArticleUrl(_headerMenu.Name, Next.MaxBehotTime, Next.MinBehotTime, maxCreateTime);

                HtmlParseHelper.HttpGet(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// 获取推荐的数据
        /// </summary>
        private void GetRecommend()
        {
            ReceiveDatas.Clear();

            string url = "http://toutiao.com/api/article/recent/?category=__all__&count=20&utm_source=toutiao&max_behot_time=1405239965.73";

            HtmlParseHelper = new HtmlParseHelper(ReceiveDatas, Next, FooterGrid, ProgressBar);
            HtmlParseHelper.HttpGet(url);
        }

        /// <summary>
        /// 初始化状态栏
        /// </summary>
        private void InitStatusBar()
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
        }

        #endregion

        void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
            
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);

            DataListView.SelectedItem = null;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            foreach (ReceiveData receiveData in ReceiveDatas)
            {
                receiveData.IsAbstract = SettingsHelper.GetAbstractState();
            }

        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Application.Current.Exit();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        #endregion

        
      
        /// <summary>
        /// 点击列表时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = DataListView.SelectedItem as ReceiveData;
            if (item == null)
            {
                return;
            }
            Frame.Navigate(typeof(ItemBrowsePage), item);
        }

        private void DataListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            MyHeaderControl.RefreshImage.Tapped += RefreshImage_Tapped;
        }

        void RefreshImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ReceiveListData();
        }

        private void ReceiveListData()
        {
            //MyHeaderControl.RefreshImageStoryboard.Begin();
            //if (_headMenuListBox.SelectedIndex == 0)
            //{
            //    GetRecommend();
            //}
            //else
            //{
                ReceiveDatas.Clear();
                Next.MinBehotTime = null;
                Next.MaxBehotTime = null;
                ReloadData();
            //}
        }

        private void FavoriteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FavoritePage));
        }

        private void AboutAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (AboutPage));

        }

        private void SettingsAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SettingsPage), this);
        }


        private bool ExistItem(ReceiveData receiveData)
        {
            var tempItem = App.FavoriteDatas.Find(n => n.Id == receiveData.Id);
            return tempItem != null;
        }

        private async void SaveFavorite(ReceiveData receiveData)
        {
            if (!ExistItem(receiveData))
                App.FavoriteDatas.Add(receiveData);
            await LocalFileHelper.Save(ToutiaoHelper.FILE_NAME, App.FavoriteDatas);
            ToutiaoHelper.ShowMessage("已添加到收藏，请到“我的收藏”中查看！", MessageTextBlock, MyFlyout, this);
        }

        private void AddItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            long id = menuItem != null && menuItem.Tag is long ? (long)menuItem.Tag : 0;

            var item  = ReceiveDatas.FirstOrDefault(n => n.Id == id);
            SaveFavorite(item);
        }

        private void UIElement_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

      

        #region utils

        
        #endregion

        private void ClosePopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values["popuped"] = PopupCheckBox.IsChecked;
            popup.IsOpen = false;
            CommandBar.Visibility = Visibility.Visible;
        }
    }
}
