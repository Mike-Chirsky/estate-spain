using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog.Es
{
    public class CategoryTreeItem
    {
        public CategoryTreeItem()
        {
            this.Items = new List<CategoryTreeItem>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string SystemName { get; set; }
        public string Slug { get; set; }
        public string SeoPath { get; set; }
        public string Url { get; set; }
        public int TotalCount { get; set; }
        public List<CategoryTreeItem> Items { get; set; }
        public bool Expanded { get; set; }
        public string Filter { get; set; }
        public string OriginalUrl { get; set; }
    }
}
