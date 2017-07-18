using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog.Extensions
{
    public class ProductFilterCriteria
    {        
        public string City { set; get; }
        
        public string Condition { set; get; }

        public string Region { set; get; }

        public string EstateType { set; get; }

        public string DisToSea { set; get; }

        public string Bath { set; get; }

        public string Price { set; get; }

        /// <summary>
        /// Bed rooms
        /// </summary>
        public string Broom { set; get; }

        /// <summary>
        /// Land square
        /// </summary>
        public string Ls { set; get; }

        /// <summary>
        /// Other type
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// Square
        /// </summary>
        public string Sq { set; get; }

        /// <summary>
        /// Fiter by sys_filter
        /// </summary>
        public string More { set; get; }
    }
}
