using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Core;
using Windows.UI.Xaml;
using jinritoutiao.Core.Model;

namespace jinritoutiao.Core
{
    public class HtmlParseHelper : HtmlHelper
    {
        private CoreDispatcher _simpleDispatcher;
        public ObservableCollection<ReceiveData> ReceiveDatas { get; set; }

        public HtmlParseHelper(ObservableCollection<ReceiveData> receiveDatas)
        {
            _simpleDispatcher = Window.Current.Dispatcher;
            ReceiveDatas = receiveDatas;
        }

        protected override void HtmlHandler(string html)
        {
            Debug.WriteLine(html);

            var jsonObject = JsonObject.Parse(html);
            var message = jsonObject["message"].GetString();

            if (message.Equals("success"))  // 返回数据成功！
            {
                var dataArray = jsonObject["data"].GetArray();
                var next = jsonObject["next"].GetObject();


                _simpleDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    foreach (var itemValue in dataArray)
                    {
                        JsonObject itemObject = itemValue.GetObject();
                        string title = itemObject["title"].GetString();
                        string sourceUrl = itemObject["source_url"].GetString();

                        var images = new string[3];
                        
                        var imageList = itemObject["image_list"].GetArray();

                        if(imageList.Count == 1)
                        {
                            images[0] = imageList[0].ToString();
                            images[1] = imageList[1].ToString();
                        }
                        else if (imageList.Count >= 2)
                        {
                            images[0] = imageList[0].ToString();
                            images[1] = imageList[1].ToString();
                            images[2] = imageList[2].ToString();
                        }
                        

                        ReceiveDatas.Add(new ReceiveData()
                        {
                            Title = title,
                            SourceUrl = sourceUrl,
                            ImageList = images
                        });
                    }
                });
            }
        }
    }
}
