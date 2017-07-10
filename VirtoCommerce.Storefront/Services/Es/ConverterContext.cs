namespace VirtoCommerce.Storefront.Services.Es
{
    using System;
    using VirtoCommerce.Storefront.Model.Catalog;

    public class ConverterContext
    {
        public Category Parent { get; set; }
        public string ProductType { get; set; }

        public string Path { get; set; }
        public string CurrentLanguage { get; set; }
    }
}