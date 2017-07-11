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

        public string Type { set; get; }

        public string CityUrl { set; get; }
        public string RegionUrl { set; get; }
        public string CityName { set; get; }
        public string RegionName { set; get; }

    }
}
