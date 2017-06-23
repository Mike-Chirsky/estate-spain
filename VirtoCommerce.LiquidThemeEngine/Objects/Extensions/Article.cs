using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Article
    {
        public IList<string> SliderImages { set; get; } = new List<string>();
    }
}
