using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace jinritoutiao.Core
{
    public abstract class HtmlHelper
    {
        private CoreDispatcher _simpleDispatcher; // 刷新UI用
        private static string HtmlString { get; set; }

        public void HttpGet(string url)
        {
            try
            {
                Debug.WriteLine(url);
                _simpleDispatcher = Window.Current.Dispatcher;
                //创建WebRequest类
                var request = WebRequest.CreateHttp(new Uri(url));
                //设置请求方式GET POST
                request.Method = "GET";
                //返回应答请求异步操作的状态                
                request.BeginGetResponse(ResponseCallback, request);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        private void ResponseCallback(IAsyncResult result)
        {
            try
            {
                //获取异步操作返回的的信息
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                //结束对 Internet 资源的异步请求
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

                //应答头信息验证
                using (Stream stream = response.GetResponseStream())
                {
                    //获取请求信息
                    StreamReader read = new StreamReader(stream);
                    HtmlString = read.ReadToEnd();
                    HtmlHandler(HtmlString);
                }

            }
            catch (WebException e)
            {
                _simpleDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MessageDialog messageDialog = new MessageDialog("请检查您的网络连接！", "错误");
                    messageDialog.ShowAsync();
                });
            }

        }

        /// <summary>
        /// 获取html后处理(需要实现)
        /// </summary>
        /// <param name="html"></param>
        protected abstract void HtmlHandler(string html);
    }
}
