using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            InitHeaderMenu();
            InitConfig();

        }

        /// <summary>
        /// 初始化配置信息
        /// </summary>
        private async void InitConfig()
        {
            

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
                ReceiveDatas.Clear();
                Next.MinBehotTime = null;
                Next.MaxBehotTime = null;
                ReloadData();
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
                    maxCreateTime = ReceiveDatas.LastOrDefault().CreateTime;
                }



                string url = ToutiaoHelper.GetArticleUrl(_headerMenu.Name, Next.MaxBehotTime, Next.MinBehotTime, maxCreateTime);
                Debug.WriteLine(url);
                HtmlParseHelper.HttpGet(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            MyHeaderControl.RefreshImage.Tapped += RefreshImage_Tapped;

           
        }

        void RefreshImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ReceiveDatas.Clear();
            Next.MinBehotTime = null;
            Next.MaxBehotTime = null;
            ReloadData();
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
    }
}
