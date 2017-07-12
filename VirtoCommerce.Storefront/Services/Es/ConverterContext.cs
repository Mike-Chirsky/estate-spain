namespace VirtoCommerce.Storefront.Services.Es
{
    using System;
    using System.Collections.Generic;
    using VirtoCommerce.Storefront.Model.Catalog;

    public class ConverterContext
    {
        public List<Product> ListExceptions { set; get; }
        public Category Parent { get; set; }
        public string ProductType { get; set; }

        public string Path { get; set; }
        public string CurrentLanguage { get; set; }
    }
}