using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using jinritoutiao.Core.Model;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace jinritoutiao.Core
{
    /// <summary>
    /// 项目工具类
    /// </summary>
    public sealed class ToutiaoHelper
    {
        private static string _articleUrl = "http://toutiao.com/api/article/recent/?category={0}&count=20&utm_source=toutiao";
        private const string FILE_NAME = "favorite";
        /// <summary>
        /// 获取文章url
        /// </summary>
        /// <param name="category"></param>
        /// <param name="maxBehotTime"></param>
        /// <param name="minBehotTime"></param>
        /// <returns></returns>
        public static string GetArticleUrl(string category, string maxBehotTime, string minBehotTime, string maxCreateTime)
        {
            string url = _articleUrl;
            if (!string.IsNullOrEmpty(maxBehotTime) && !string.IsNullOrEmpty(maxBehotTime))
            {
                url += "&max_behot_time=" + maxBehotTime + "&min_behot_time=" + minBehotTime;
            }
            if (!string.IsNullOrEmpty(maxCreateTime))
            {
                url += "&max_create_time=" + maxCreateTime;
            }
            url += "_=" + DateTime.Now.Ticks;
            return string.Format(url, category);
        }

        /// <summary>
        /// 获取顶部菜单
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<HeaderMenu> GetHeaderMenus()
        {
            return new ObservableCollection<HeaderMenu>()
            {
                new HeaderMenu(){Name = "__all__", Title = "推荐"},
                new HeaderMenu(){Name = "news_hot", Title = "热点"},
                new HeaderMenu(){Name = "news_society", Title = "社会"},
                new HeaderMenu(){Name = "news_entertainment", Title = "娱乐"},
                new HeaderMenu(){Name = "news_finance", Title = "财经"},
                new HeaderMenu(){Name = "news_military", Title = "军事"},
                new HeaderMenu(){Name = "news_tech", Title = "科技"},
                new HeaderMenu(){Name = "news_world", Title = "国际"},
                new HeaderMenu(){Name = "news_sports", Title = "体育"},
                new HeaderMenu(){Name = "news_car", Title = "汽车"},
                new HeaderMenu(){Name = "news_travel", Title = "旅游"},
                new HeaderMenu(){Name = "news_regimen", Title = "养生"},
                new HeaderMenu(){Name = "news_history", Title = "历史"},
                new HeaderMenu(){Name = "news_discovery", Title = "探索"},
                new HeaderMenu(){Name = "news_story", Title = "故事"},
                new HeaderMenu(){Name = "news_essay", Title = "美文"}
            };
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <returns></returns>
        public async static Task SaveFavorite()
        {
            string temp = "";
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<ReceiveData>));
                ser.Serialize(sw, App.FavoriteDatas);
                temp = sw.ToString();
                sw.Dispose();
            }
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.OpenIfExists);
            FileIO.WriteTextAsync(file, temp, UnicodeEncoding.Utf8);
            Debug.WriteLine(string.Format("Favorite count:{0}", App.FavoriteDatas.Count));
        }

        public async static Task<List<ReceiveData>> ReadFavorite()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            List<ReceiveData> tempList = new List<ReceiveData>();
            try
            {
                StorageFile file = await folder.GetFileAsync(FILE_NAME);
                string text = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
                Debug.WriteLine(text);
                using (StringReader rdr = new StringReader(text))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<ReceiveData>));
                    tempList = (List<ReceiveData>)ser.Deserialize(rdr);
                    rdr.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return tempList;
        }
    }
}
