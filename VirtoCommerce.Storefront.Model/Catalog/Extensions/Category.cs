using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    partial class Category
    {
        public string ProductType { get; set; }
        public string Path { get; set; }

        public string Description { get; set; }

        public ICollection<EditorialReview> Descriptions { get; set; }

        public Category Parent { get; set; }

        public string FullName { get; set; }

        /// <summary>
        /// Info for seo Region
        /// </summary>
        public Product RegionProduct { set; get; }

        /// <summary>
        /// Info for seo City
        /// </summary>
        public Product CityProduct { set; get; }

        /// <summary>
        /// Info for type product
        /// </summary>
        public Product TypeProduct { set; get; }
    }
}
