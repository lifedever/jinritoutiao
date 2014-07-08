﻿using System;
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
                        var itemJson = itemValue.Stringify();
                        JsonObject itemObject = itemValue.GetObject();
                        // 存在Image_list，可显示多个图片
                        string[] imageArray = new string[3];
                        int imageCount = 0;

                        if (itemJson.Contains("image_url"))
                        {
                            imageArray[0] = itemObject["image_url"].GetString();
                            imageCount = 1;
                        }
                        else if (itemJson.Contains("image_list"))
                        {
                            var images = itemObject["image_list"].GetArray();
                            if (images.Count == 1)  //一张图片
                            {
                                imageArray[0] = images[0].GetObject()["url"].GetString();
                                imageCount = 1;
                            }
                            else if (images.Count == 2)
                            {
                                imageArray[0] = images[0].GetObject()["url"].GetString();
                                imageArray[1] = images[1].GetObject()["url"].GetString();
                                imageCount = 2;
                            }
                            else if (images.Count == 3)
                            {
                                imageArray[0] = images[0].GetObject()["url"].GetString();
                                imageArray[1] = images[1].GetObject()["url"].GetString();
                                imageArray[2] = images[2].GetObject()["url"].GetString();
                                imageCount = 3;
                            }
                        }


                        string title = itemObject["title"].GetString();
                        string sourceUrl = itemObject["source_url"].GetString();
                        string source = itemObject["source"].GetString();
                        int commentsCount = (int) itemObject["comments_count"].GetNumber();
                        string datetime = itemObject["datetime"].GetString();
                        ReceiveDatas.Add(new ReceiveData()
                        {
                            Title = title,
                            SourceUrl = sourceUrl,
                            ImageList = imageArray,
                            ImageCount = imageCount,
                            Source = source,
                            CommentsCount = commentsCount,
                            Datetime = datetime
                        });
                    }
                });
            }
        }
    }
}
