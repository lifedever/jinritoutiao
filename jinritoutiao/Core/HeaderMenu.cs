using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jinritoutiao.Core
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class HeaderMenu
    {
        private string _name;
        private string _title;


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
