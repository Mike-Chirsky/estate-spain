using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Filters;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Product
    {

        #region Properties

        /// <summary>
        /// Get H1 property
        /// </summary>
        public string H1
        {
            get
            {
                return GetPropertyByName("h1")?.Value;
            }
        }
        /// <summary>
        /// Get estate type
        /// </summary>
        public string Estatetype
        {
            get
            {
                return GetPropertyByName("estatetype")?.Value;
            }
        }
        /// <summary>
        /// Get deal type 
        /// </summary>
        public string DealType
        {
            get
            {
                return GetPropertyByName("dealtype")?.Value;
            }
        }
        /// <summary>
        /// Get available
        /// </summary>
        public string AvailableProp
        {
            get
            {
                return GetPropertyByName("available")?.Value;
            }
        }

        /// <summary>
        /// Get other type 
        /// </summary>
        public string OtherType
        {
            get
            {
                return GetPropertyByName("other_type")?.Value;
            }
        }

        /// <summary>
        /// Count rooms in lot
        /// </summary>
        public string CountRooms
        {
            get
            {
                return GetPropertyByName("rooms")?.Value;
            }
        }
        /// <summary>
        /// Count bath rooms in lot
        /// </summary>
        public string CountBathRooms
        {
            get
            {
                return GetPropertyByName("bath")?.Value;
            }
        }
        /// <summary>
        /// Count bed rooms in lot
        /// </summary>
        public string CountBedRooms
        {
            get
            {
                return GetPropertyByName("bedrooms")?.Value;
            }
        }
        /// <summary>
        /// Display square
        /// </summary>
        public string Square
        {
            get
            {
                return GetSquare();
            }
        }
        /// <summary>
        /// Get property value
        /// </summary>
        public string PropertySquare {
            get
            {
                return GetRoudedSquareProperty("propertysquare");
            }
        }

        /// <summary>
        /// Get property value
        /// </summary>
        public string LandSquare
        {
            get
            {
                return GetRoudedSquareProperty("landsquare");
            }
        }

        /// <summary>
        /// Get property value
        /// </summary>
        public string TerraceSquare
        {
            get
            {
                return GetRoudedSquareProperty("terracesquare");
            }
        }

        /// <summary>
        /// Region
        /// </summary>
        public string Region
        {
            get
            {
                var vl = GetPropertyByName("regions")?.Value;
                if (string.IsNullOrEmpty(vl))
                {
                    return null;
                }
                var paths = vl.Split('/');
                if (paths.Length > 1)
                {
                    return paths[1];
                }
                else
                {
                    return paths.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Region
        /// </summary>
        public string City
        {
            get
            {
                return GetPropertyByName("city")?.Value;
            }
        }

        /// <summary>
        /// District
        /// </summary>
        public string District
        {
            get
            {
                return GetPropertyByName("district")?.Value;
            }
        }
        #endregion

        /// <summary>
        /// Get small product description
        /// </summary>
        public string SmallDescription
        {
            get
            {
                return GetDescriptionByType("small-description");
            }
        }

        /// <summary>
        /// Get detail product description
        /// </summary>
        public string ProductDescription
        {
            get
            {
                return GetDescriptionByType("full-description");
            }
        }

        /// <summary>
        /// Get seo text down
        /// </summary>
        public string SeoTextDown1
        {
            get {
                return GetDescriptionByType("seotext-down1");
            }
        }

        /// <summary>
        /// Custom title
        /// </summary>
        public string CustomTitle
        {
            get
            {
                return GetCustomTitle();
            }
        }

        /// <summary>
        /// Custom title
        /// </summary>
        public string CustomTitleWithoutHtml
        {
            get
            {
                return GetCustomTitle(false);
            }
        }
        /// <summary>
        /// Get breadcrumb title
        /// </summary>
        public string BreadcrumbTitle
        {
            get
            {
                return GetBreadcrumbTitle();
            }
        }
        /// <summary>
        /// Get custom title for product in listing
        /// </summary>
        public string ListingCustomTitle
        {
            get
            {
                return GetCustomTitle(includeCity: false);
            }
        }

        public ICollection<KeyValue<string, string>> Breadcrumb { set; get; }

        /// <summary>
        /// Get title for breadcrumb
        /// </summary>
        /// <returns></returns>
        private string GetBreadcrumbTitle()
        {
            if (Properties.Count == 0)
            {
                return Title;
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

            if (FirstAvailableVariant != null)
            {
                title += $"№{FirstAvailableVariant.Sku} ";
            }
            var square = GetSquare();
            if (!string.IsNullOrEmpty(square))
            {
                title += $"{square} м2";
            }
            return title;
        }


        /// <summary>
        /// Get custom title for product
        /// </summary>
        /// <param name="withHtml">Use html tags in return result</param>
        /// <returns></returns>
        private string GetCustomTitle(bool withHtml = true, bool includeCity = true)
        {
            if (Properties.Count == 0)
            {
                return Title;
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

            if (FirstAvailableVariant != null)
            {
                title += $"{TranslationFilter.T("product.number")}{FirstAvailableVariant.Sku} ";
            }
            var square = GetSquare();
            if (!string.IsNullOrEmpty(square))
            {
                title += $"{square} {TranslationFilter.T(withHtml ? "product.square" : "product.square2")} ";
            }
            if (includeCity)
            {
                var propertyCity = GetPropertyByName("city_name_in");
                if (propertyCity == null || string.IsNullOrEmpty(propertyCity.Value))
                {
                    propertyCity = GetPropertyByName("city");
                }
                if (propertyCity != null && !string.IsNullOrEmpty(propertyCity.Value))
                {
                    title += $"{TranslationFilter.T("product.in-city")} {propertyCity.Value}";
                }
            }
            return title;
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

        private bool IsNullOrEmptyProperty(ProductProperty property)
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
        /// <summary>
        /// Get property by name
        /// </summary>
        private ProductProperty GetPropertyByName(string propertyName)
        {
            return Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get rounded square property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private string GetRoudedSquareProperty(string propertyName)
        {
            var propertySquare = GetPropertyByName(propertyName);
            if (IsNullOrEmptyProperty(propertySquare))
            {
                return null;
            }
            var roundedValue = RoundValue(propertySquare.Value);
            return roundedValue.HasValue ? roundedValue.ToString() : null;
        }

        /// <summary>
        /// Get description content by description type
        /// </summary>
        private string GetDescriptionByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return string.Empty;
            }
            return Descriptions.FirstOrDefault(x => type.Equals(x.Type, StringComparison.InvariantCultureIgnoreCase))?.Content;
        }

        private string GetImageSrcByType(string type)
        {
            var img = Images.FirstOrDefault(x => Path.GetFileName(x.Src)?.StartsWith(type, StringComparison.InvariantCultureIgnoreCase) ?? false);
            return img != null ? img.Src : "http://wpresidence.net/wp-content/uploads/2015/11/file112128225584523-980x777.jpg";
        }
    }
}
