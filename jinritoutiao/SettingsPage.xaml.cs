using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍
using jinritoutiao.Common;
using jinritoutiao.Core;
using jinritoutiao.Core.Model;

namespace jinritoutiao
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private MainPage _mainPage;
        public SettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this._navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;


            InitStatusBar();
            InitSettings();
        }
        void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {


        }
        private void InitSettings()
        {
            AbstracToggleSwitch.IsOn = SettingsHelper.GetAbstractState();
            PopupToggleSwitch.IsOn = SettingsHelper.GetPopupState();
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
            this._navigationHelper.OnNavigatedTo(e);
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            _mainPage = e.Parameter as MainPage;
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
        private void AbstracToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            SettingsHelper.ChangeAbstract(AbstracToggleSwitch.IsOn);
            if (_mainPage != null) {
                _mainPage.DataListView.ItemsSource = null;
                _mainPage.DataListView.ItemsSource = _mainPage.ReceiveDatas;
            }
        }

        private void PopupToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values["popuped"] = !PopupToggleSwitch.IsOn;
        }
    }
}
