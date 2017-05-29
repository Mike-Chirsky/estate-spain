﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Pricing.Services
{
    public interface IPricingService
    {
        Task EvaluateProductPricesAsync(ICollection<Product> products, WorkContext workContext);      
    }
}
