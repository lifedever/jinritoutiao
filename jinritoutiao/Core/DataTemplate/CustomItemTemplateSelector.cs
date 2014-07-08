using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using jinritoutiao.Core.Model;

namespace jinritoutiao.Core.dataTemplate
{
    public class CustomItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NoImageDataTemplate { get; set; }

        public DataTemplate OneImageDataTemplate { get; set; }

        public DataTemplate TwoImageDataTemplate { get; set; }

        public DataTemplate ThreeImageDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var dataItem = item as ReceiveData;
            if (dataItem != null)
            {
                if (dataItem.ImageCount == 0)
                {
                    return NoImageDataTemplate;
                }
                else if (dataItem.ImageCount == 1)
                {
                    return OneImageDataTemplate;
                }
                else if(dataItem.ImageCount == 2)
                {
                    return TwoImageDataTemplate;
                }
                else if (dataItem.ImageCount == 3)
                {
                    return ThreeImageDataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
