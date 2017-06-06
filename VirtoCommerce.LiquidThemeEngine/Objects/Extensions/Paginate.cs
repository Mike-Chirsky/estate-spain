using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Paginate
    {
        private const int CountView = 5;
        public List<Part> CustomParts
        {
            get
            {
                var listParts = new List<Part>();
                if (Pages == 0)
                    return listParts;
                var startIndex = 0;
                var count = 0;
                bool isAddLast = true;
                if (CurrentPage < CountView)
                {
                    startIndex = 0;
                    count = CountView;
                    if (CurrentPage == 1)
                        count++;
                }
                else
                {
                    startIndex = CurrentPage - 2;
                    count = CountView / 2 + 1;   
                    listParts.Add(Parts.First());
                    listParts.Add(new Part
                    {
                        Title = "..."
                    });
                }
                if (startIndex + count > Pages - 1)
                {
                    if (CurrentPage == Pages)
                        startIndex = Pages - CountView - 1;
                    else
                        startIndex = Pages - CountView;
                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                    count = Pages - startIndex;
                    isAddLast = false;
                }
                listParts.AddRange(Parts.GetRange(startIndex, count));
                if (startIndex + count < Pages - 1)
                {
                    listParts.Add(new Part
                    {
                        Title = "..."
                    });
                }
                if (isAddLast)
                {
                    listParts.Add(Parts.Last());
                }
                return listParts;
            }
        }
    }
}
