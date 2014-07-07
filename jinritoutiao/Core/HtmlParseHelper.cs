using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jinritoutiao.Core
{
    public class HtmlParseHelper : HtmlHelper
    {
        protected override void HtmlHandler(string html)
        {
            Debug.WriteLine(html);
        }
    }
}
