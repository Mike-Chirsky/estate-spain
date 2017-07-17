using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Product
    {

        public string CustomTitle
        {
            get { return GetCustomTitle(); }
        }

        public string Square
        {
            get { return GetSquare(); }
        }

        /// <summary>
        /// Get title for breadcrumb
        /// </summary>
        /// <returns></returns>
        private string GetCustomTitle()
        {
            if (Properties.Count == 0)
            {
                return Name;
            }
            var title = GetPropertyByName("other_type")?.Value;
            if (!string.IsNullOrEmpty(title))
            {
                title = $"{char.ToUpper(title.First())}{title.Substring(1)} ";
            }
            else
            {
                title = string.Empty;
            }
            title += $"№{Sku} ";
            var square = GetSquare();
            if (!string.IsNullOrEmpty(square))
            {
                title += $"{square} м2";
            }
            return title;
        }

        /// <summary>
        /// Get property by name
        /// </summary>
        private CatalogProperty GetPropertyByName(string propertyName)
        {
            return Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get rounded square by rule, if feel "propertysquare" and "landsquare" return  "propertysquare" value
        /// </summary>
        /// <returns></returns>
        private string GetSquare()
        {
            string square = null;
            var propertySquare = GetPropertyByName("propertysquare");
            var landSquare = GetPropertyByName("landsquare");
            if (!IsNullOrEmptyProperty(propertySquare))
            {
                square = propertySquare.Value;
            }
            else
            {
                if (!IsNullOrEmptyProperty(landSquare))
                {
                    square = landSquare.Value;
                }
            }
            var roundSquare = RoundValue(square);
            if (roundSquare.HasValue)
            {
                square = roundSquare.Value.ToString();
            }
            return square;
        }
        private bool IsNullOrEmptyProperty(CatalogProperty property)
        {
            return property == null || string.IsNullOrEmpty(property.Value) || property.Value.Equals("0.00", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Round value from string value
        /// </summary>
        /// <returns></returns>
        private double? RoundValue(string value)
        {
            // TODO: after move to VS17 delete
            double parseValue = 0;
            if (!string.IsNullOrEmpty(value) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parseValue))
            {
                return Math.Round(parseValue, MidpointRounding.ToEven);
            }
            return null;
        }
    }
}
