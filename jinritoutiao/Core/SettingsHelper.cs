using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace jinritoutiao.Core
{
    public class SettingsHelper
    {
        private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 显示摘要
        /// </summary>
        public static void ChangeAbstract(bool flag)
        {
            Settings.Values["abstract"] = flag;
        }

       

        /// <summary>
        /// 获得摘要状态
        /// </summary>
        /// <returns></returns>
        public static bool GetAbstractState()
        {
            if (Settings.Values.ContainsKey("abstract"))
                return (bool) Settings.Values["abstract"];
            return false;
        }


        public static bool GetPopupState()
        {
            if (Settings.Values.ContainsKey("popuped"))
                return !(bool)Settings.Values["popuped"];
            return false;
        }
    }
}
