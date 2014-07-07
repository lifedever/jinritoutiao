using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jinritoutiao.Core.Model;

namespace jinritoutiao.Core
{
    /// <summary>
    /// 项目工具类
    /// </summary>
    public sealed class ToutiaoHelper
    {
        private static string _articleUrl = "http://toutiao.com/api/article/recent/?category={0}&count=20";

        /// <summary>
        /// 获取文章url
        /// </summary>
        /// <param name="category"></param>
        /// <param name="maxBehotTime"></param>
        /// <param name="minBehotTime"></param>
        /// <returns></returns>
        public static string GetArticleUrl(string category, string maxBehotTime, string minBehotTime)
        {
            if (!string.IsNullOrEmpty(maxBehotTime) && !string.IsNullOrEmpty(maxBehotTime))
            {
                _articleUrl += string.Format("&max_behot_time={0}&min_behot_time={1}", maxBehotTime, minBehotTime);
            }

            return string.Format(_articleUrl, category);
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

    }
}
